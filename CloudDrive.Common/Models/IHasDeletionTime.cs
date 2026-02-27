namespace CloudDrive.Common.Models
{
    public interface IHasDeletionTime
    {
        DateTime? DeletionTime { get; }
    }
}