namespace Server.Core.Dtos.Task;

public record TaskGetQueryDto(
        int page,
        int pageSize,
        SortBy sortBy,
        int[]? categoryIds = null,
        Core.Status? selectedStatus = null,
        string? searchTerm = null);

// public enum Filter {
//     All,
//     Done,
//     Pending,
//     Edited,
//     Unedited,
// }

public enum SortBy {
    AlphAsc,
    AlphDsc,
    CrtAsc,
    CrtDsc,
    UpdAsc,
    UpdDsc,
}
