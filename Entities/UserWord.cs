using System;
using System.Collections.Generic;
using WordMemoryApi.Entities;


namespace WordMemoryApi.Entities
{
    public class UserWord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public int RepetitionCount { get; set; } = 0;
        public DateTime NextRepetitionDate { get; set; } = DateTime.Now;


        public User? User { get; set; }
        public Word? Word { get; set; }
    }
}
