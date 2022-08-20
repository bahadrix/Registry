using System;
using NimbleRegistry;

namespace Tests;


    public class BaseEntity : IRegistryPayload
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
