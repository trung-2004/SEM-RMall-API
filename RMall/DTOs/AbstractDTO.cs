﻿namespace RMall.DTOs
{
    public class AbstractDTO<T>
    {
        public int id { get; set; }

        public DateTime? createdAt { get; set; }

        public DateTime? updatedAt { get; set; }

        public DateTime? deletedAt { get; set; }
    }
}
