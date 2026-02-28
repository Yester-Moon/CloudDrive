namespace CloudDrive.Common.Models
{
    public record AggregateRootEntity : BaseEntity, IAggregateRoot, ISoftDelete, IHasCreationTime, IHasDeletionTime, IHasModificationTime
    {
        public bool IsDeleted { get; private set; }
        public DateTime CreationTime { get; private set; } = DateTime.Now;
        public DateTime? DeletionTime { get; private set; }
        public DateTime? LastModificationTime { get; private set; }

        public virtual void SoftDelete()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
        }

        /// <summary>
        /// 从软删除状态恢复
        /// </summary>
        public virtual void Restore()
        {
            this.IsDeleted = false;
            this.DeletionTime = null;
            this.LastModificationTime = DateTime.Now;
        }

        public void NotifyModified()
        {
            this.LastModificationTime = DateTime.Now;
        }
    }
}