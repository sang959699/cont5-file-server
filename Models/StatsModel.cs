using System.Collections.Generic;

namespace Cont5.Models.Stats {
    public class GetStatsModel {
        public GetStatsModel() {
            Data = new List<string[]>();
        }
        public List<string[]> Data { get; set; }
    }
}