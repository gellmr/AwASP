namespace AngularWithASP.Server.Domain.Abstract
{
  public interface ICartLineRepository
  {
    CartLine? SaveCartLine(CartLine cartLine);
    void ClearCartLines(Nullable<Guid> guestID);
    void ClearUserCartLines(string? uid);
    void RemoveById(Int32 cartLineIdRem);
    IEnumerable<CartLine> CartLines { get; }
    public IEnumerable<CartLine> WithIsps(IEnumerable<CartLine> lines);
    public Task MergeGuestCartIntoUserAsync(Guid guestId, string userId);
  }
}

