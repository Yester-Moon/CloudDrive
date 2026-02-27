namespace CloudDrive.Common.Models
{
    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; }
    }
}