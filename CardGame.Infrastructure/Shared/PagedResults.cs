namespace CardGame.Infrastructure.Shared;

public class PagedResults<T> where T : class  // we will only use this for classes, not structs or primitives
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; } = 10; //default page size
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
