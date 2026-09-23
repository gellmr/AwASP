using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Domain;
using AngularWithASP.Server.Domain.Abstract;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;

namespace AngularWithASP.Server.Infrastructure
{
  public class MyAddressDto
  {
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; } = "Australia";
    public string? Zip { get; set; }

    public static Address ToAddress(MyAddressDto dto)
    {
      return new Address
      {
        Line1 = dto.Line1,
        Line2 = dto.Line2,
        Line3 = dto.Line3,
        City = dto.City,
        State = dto.State,
        Country = dto.Country,
        Zip = dto.Zip
      };
    }
  }

  public class AddressParser
  {
    IOrdersRepository ordersRepository { get; set; }
    private StoreContext context;

    private List<string> billAddresses = new List<string>();
    private List<string> shipAddresses = new List<string>();

    public AddressParser(IOrdersRepository oRepo, StoreContext ctx){
      ordersRepository = oRepo;
      context = ctx;
    }

    public async Task Execute()
    {
      await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
      try
      {
        IEnumerable<UOrder> allorders = await ordersRepository.GetAllOrdersAsync();
        foreach (OrderV1 ov1 in allorders)
        {
          MyAddressDto billDto = ParseAddress(ov1.BillingAddress);
          MyAddressDto shipDto = ParseAddress(ov1.ShippingAddress);
          Order ov2 = new Order(ov1);
          ov2.BillAddress = MyAddressDto.ToAddress(billDto);
          ov2.ShipAddress = MyAddressDto.ToAddress(shipDto);
          await ordersRepository.SaveOrderAsync(ov2);
        }
        await transaction.CommitAsync();
        Console.WriteLine("Transaction committed successfully.");
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        Console.WriteLine("Error during AddressParser.Execute... Transaction rolled back." + ex.ToString());
      }
    }

    public List<MyAddressDto> ParseAddresses(List<string> addresses)
    {
      var result = new List<MyAddressDto>();
      foreach (var address in addresses){
        result.Add(ParseAddress(address));
      }
      return result;
    }

    public MyAddressDto ParseAddress(string address)
    {
      var stateRegex = new Regex(@"\b(WA|SA|NSW|VIC|QLD|NT|TAS|ACT)\b", RegexOptions.IgnoreCase);
      var zipRegex = new Regex(@"\b\d{4}\b");

      var dto = new MyAddressDto();

      // Extract Zip (4 digits at the end) and State
      var zipMatch = zipRegex.Matches(address).LastOrDefault();
      if (zipMatch != null)
      {
        dto.Zip = zipMatch.Value;
      }

      var stateMatch = stateRegex.Matches(address).LastOrDefault();
      if (stateMatch.Success)
      {
        dto.State = stateMatch.Value.ToUpper();
      }

      // Remove Zip and State from the string
      var cleanAddress = address;

      if (dto.Zip != null)
      {
        int lastIndex = cleanAddress.LastIndexOf(dto.Zip);
        if (lastIndex >= 0)
        {
          cleanAddress = cleanAddress.Remove(lastIndex, dto.Zip.Length).Trim();
        }
      }

      if (dto.State != null)
      {
        int lastIndex = cleanAddress.LastIndexOf(dto.State, StringComparison.OrdinalIgnoreCase);
        if (lastIndex >= 0)
        {
          cleanAddress = cleanAddress.Remove(lastIndex, dto.State.Length).Trim();
        }
      }

      // Split the remaining string by commas and spaces to find city and lines
      var parts = cleanAddress.Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(p => p.Trim())
                               .Where(p => !string.IsNullOrEmpty(p))
                               .ToList();

      if (parts.Any())
      {
        // The last part is the City
        dto.City = parts.Last();
        parts.RemoveAt(parts.Count - 1);
      }

      // Assign Lines
      if (parts.Count > 0)
      {
        dto.Line1 = parts[0];
      }
      if (parts.Count > 1)
      {
        dto.Line2 = parts[1];
      }
      if (parts.Count > 2)
      {
        dto.Line3 = string.Join(", ", parts.Skip(2));
      }
      return dto;
    }
  }
}
