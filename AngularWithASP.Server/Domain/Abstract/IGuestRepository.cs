using Microsoft.EntityFrameworkCore.Storage;
using AngularWithASP.Server.DTO;

namespace AngularWithASP.Server.Domain.Abstract
{
  public interface IGuestRepository
  {
    IQueryable<Guest> Guests { get; }
    Task<IDbContextTransaction> BeginTransactionAsync();
    void SaveGuest(Guest guest);
    Nullable<Guid> GuestExists(string email);
    Task<Guest?> UpdateWithTransaction(GuestUpdateDTO dto);
  }
}
