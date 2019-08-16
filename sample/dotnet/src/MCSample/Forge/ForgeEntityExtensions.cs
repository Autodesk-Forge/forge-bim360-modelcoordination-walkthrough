using System;

namespace MCSample.Forge
{
    public static class ForgeEntityExtensions
    {
        public static StorageId ToStorageId(this ForgeEntity entity)
        {
            if (entity.Type != ForgeEntityType.StorageObject)
            {
                throw new InvalidOperationException($"{entity.Type} is an not a StorageObject");
            }

            return StorageId.Parse(entity.Id);
        }
    }
}
