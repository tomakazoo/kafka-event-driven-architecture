using System;
using System.Text.Json;

namespace LogCompaction.Domain
{
    /// <summary>
    /// User aggregate - represents user state
    /// Key: userId (for log compaction)
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static User FromJson(string json)
        {
            return JsonSerializer.Deserialize<User>(json) ?? new User();
        }
    }
}


