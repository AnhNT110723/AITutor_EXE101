using EXE_FAIEnglishTutor.Models;
using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class SituationFilterViewModel
    {
        public List<Situation> Items { get; set; } = new();
        public string? Search { get; set; }
        public int? LevelId { get; set; }
        public int? TypeId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 10));
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
