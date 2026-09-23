using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Controllers;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Domain
{
  public class EFCartLineRepository: ICartLineRepository
  {
    private readonly IConfiguration _config;
    private StoreContext context;

    public EFCartLineRepository(IConfiguration c){
      _config = c;
      context = new StoreContext(_config);
    }

    public IEnumerable<CartLine> CartLines{
      get{
        IEnumerable<CartLine> cartLines = context.CartLines;
        return cartLines;
      }
    }

    // Chainable in lambda expression. Load the InStockProducts associated with each CartLine.
    public IEnumerable<CartLine> WithIsps(IEnumerable<CartLine> lines){
      foreach (CartLine c in lines){
        c.InStockProduct = context.InStockProducts.FirstOrDefault(p => p.ID == c.InStockProductID);
        yield return c;
      }
    }

    protected void ClearLines(IEnumerable<CartLine> lines)
    {
      foreach (CartLine c in lines){
        context.CartLines.Remove(c);
      }
      context.SaveChanges();
    }

    public void ClearUserCartLines(string? uid)
      {
        IEnumerable<CartLine> lines = context.CartLines.Where(line => line.UserID != null && (line.AppUser.Id == uid));
        ClearLines(lines);
      }

      public async Task MergeGuestCartIntoUserAsync(Guid guestId, string userId)
      {
        var guestCartLines = await context.CartLines.Where(c => c.GuestID == guestId).ToListAsync();
        foreach(var line in guestCartLines) {
          line.GuestID = null;
          line.Guest = null;
          line.UserID = userId;
        }
        await context.SaveChangesAsync();
      }

    public void ClearCartLines(Nullable<Guid> guestID)
    {
      IEnumerable<CartLine> lines = context.CartLines.Where(line => line.Guest != null && (line.Guest.ID == guestID));
      ClearLines(lines);
    }

    public void RemoveById(Int32 cartLineIdRem)
    {
      CartLine line = context.CartLines.FirstOrDefault(record => record.ID == cartLineIdRem);
      if (line != null){
        context.CartLines.Remove(line);
        context.SaveChanges();
      }
    }

    private enum UpdateAction { Create, Update, Delete, None };
    private enum UserType { AppUser, Guest, None }

    public CartLine? SaveCartLine(CartLine cartLine)
    {
      CartLine? existingCartLine = null;
      UpdateAction action = UpdateAction.None;
      UserType userType = (cartLine.UserID != null) ? UserType.AppUser : ((cartLine.GuestID != null) ? UserType.Guest : UserType.None);

      if (cartLine.ID == null){
        action = UpdateAction.Create;
      }
      else
      {
        // Update / Delete

        // Lookup the existing CartLine by (ID, InStockProductID, and Guest/AppUser)
        switch (userType)
        {
          case UserType.AppUser:
            existingCartLine = context.CartLines.FirstOrDefault(record =>
              record.ID == cartLine.ID &&
              record.UserID == cartLine.UserID &&
              record.InStockProductID == cartLine.InStockProductID
            );
            break;
          case UserType.Guest:
            existingCartLine = context.CartLines.FirstOrDefault(record =>
              record.ID == cartLine.ID &&
              record.GuestID == cartLine.GuestID &&
              record.InStockProductID == cartLine.InStockProductID
            );
            break;
          case UserType.None:
            throw new ArgumentException("Cannot look up CartLine. There is no GuestID / UserID");
            break;
        }
        if (existingCartLine != null)
        {
          // Load associated entities
          existingCartLine.InStockProduct = context.InStockProducts.FirstOrDefault(isp => isp.ID == existingCartLine.InStockProductID);
          existingCartLine.Guest = context.Guests.FirstOrDefault(g => g.ID == cartLine.GuestID);
        }
        // Set the quantity to zero, to delete row from database.
        action = (cartLine.Quantity == 0) ? UpdateAction.Delete : UpdateAction.Update;
      }

      switch (action)
      {
        case UpdateAction.Create:
          context.CartLines.Add(cartLine); // The cartLine.ID must be null when we are creating, or the DB will complain.

          // Set Unchanged for associated entities
          InStockProduct isp = cartLine.InStockProduct;
          context.Entry(isp).State = EntityState.Unchanged;   // Dont create InStockProduct. It already exists in database.

          if (cartLine.Guest != null){
            Guest? guest = cartLine.Guest;
            context.Entry(guest).State = EntityState.Unchanged; // Dont create Guest. It already exists in database.
          }

          if (cartLine.AppUser != null){
            AppUser? appUser = cartLine.AppUser;
            context.Entry(appUser).State = EntityState.Unchanged; // Dont create AppUser. Already exists.
          }

          context.SaveChanges();
          return cartLine; // Return the updated record.
          break;

        case UpdateAction.Update:
          existingCartLine.Quantity = cartLine.Quantity;

          Int32 updatedCartLineID = (Int32)existingCartLine.ID;        // The CartLine ID of the record we just updated.
          Int32 updatedIsp = (Int32)existingCartLine.InStockProductID; // The InStockProductID of the CartLine we just updated.

          // Remove any duplicate CartLines for this InStockProductID, which belong to the Guest.
          IList<CartLine> duplicateIsps = context.CartLines.Where(line =>
            (line.ID != updatedCartLineID) &&
            (line.GuestID == cartLine.GuestID) &&
            (line.InStockProductID == updatedIsp)
          ).ToList();
          context.CartLines.RemoveRange(duplicateIsps);

          context.SaveChanges();
          return existingCartLine; // Return the updated record.
          break;

        case UpdateAction.Delete:
          context.CartLines.Remove(existingCartLine);
          context.SaveChanges();
          return null; // The CartLine was successfully deleted from database.
          break;
      }
      throw new ArgumentException("Could not save CartLine");
    }
  }
}

