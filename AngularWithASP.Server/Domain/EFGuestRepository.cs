using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AngularWithASP.Server.Domain.Abstract;
using AngularWithASP.Server.DTO;
using AngularWithASP.Server.Infrastructure;
using System.Data;

namespace AngularWithASP.Server.Domain
{
  public class EFGuestRepository: IGuestRepository
  {
    private readonly IConfiguration _config;
    private StoreContext context;

    public EFGuestRepository(IConfiguration c){
      _config = c;
      context = new StoreContext(_config);
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync(){
      return await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
    }

    // Must use IQueryable to support deferred execution in async methods
    public IQueryable<Guest> Guests
    {
      get
      {
        IQueryable<Guest> guests = context.Guests.Where(g => g.ID != null); // Email, FirstName and LastName are allowed to be null.
        return guests;
      }
    }

    public void SaveGuest(Guest guest)
    {
      if (context.Guests.Any(g => g.ID == guest.ID))
      {
        // Record already exists. Update
        Guest dbEntry = context.Guests.FirstOrDefault(g => g.ID == guest.ID);
        dbEntry.ID = guest.ID;
        dbEntry.Email = guest.Email;
        dbEntry.FirstName = guest.FirstName;
        dbEntry.LastName = guest.LastName;
        dbEntry.Picture = guest.Picture;
        context.SaveChanges();
      }else{
        // Create new record
        context.Guests.Add(guest);
        context.SaveChanges();
      }
    }

    public Nullable<Guid> GuestExists(string email)
    {
      Guest guest = context.Guests.FirstOrDefault(g => g.Email.Equals(email));
      if (guest != null)
      {
        return (Nullable<Guid>)guest.ID;
      }
      return null;
    }

    private async Task<Guest?> _UpdateWithTransaction(GuestUpdateDTO dto)
    {
      var strategy = context.Database.CreateExecutionStrategy();
      return await strategy.ExecuteAsync(async () =>
      {
        // Begin the transaction inside the retriable block.
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        Guest? guest = await Guests.FirstOrDefaultAsync(g => g.ID.Equals(dto.ID));
        if (guest == null)
        {
          guest = new Guest
          {
            ID = dto.ID,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Picture = dto.Picture,
          };
          context.Guests.Add(guest);
        }
        else
        {
          guest.Email = dto.Email ?? guest.Email;
          guest.FirstName = dto.FirstName ?? guest.FirstName;
          guest.LastName = dto.LastName ?? guest.LastName;
          guest.Picture = dto.Picture ?? guest.Picture;
        }
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return guest; // Return the final result from the lambda.
      });
    }

    public async Task<Guest?> UpdateWithTransaction(GuestUpdateDTO dto)
    {
      try{
        Guest? guest = await _UpdateWithTransaction(dto);
        return guest;
      }
      catch (Exception ex){
        Console.WriteLine($" (UpdateWithTransaction) Fatal exception - retries exhausted. Msg: {ex.Message}");
        throw;
      }
    }
  }
}
