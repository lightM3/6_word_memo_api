using System;
using System.Collections.Generic;
using WordMemoryApi.Entities;


namespace WordMemoryApi.Entities

{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();

        public ICollection<QuizResult>? QuizResults { get; set; }

    }
}
