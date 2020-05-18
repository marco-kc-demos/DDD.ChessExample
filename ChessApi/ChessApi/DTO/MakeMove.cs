using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ChessApi.DTO
{
    public class MakeMove
    {
        [RegularExpression("^([RNBQK]?[a-h][1-8][x-][a-h][1-8](=[RNBQ])?|0-0-0|0-0)[+#]?$")]
        public string Notation { get; set; }
    }
}
