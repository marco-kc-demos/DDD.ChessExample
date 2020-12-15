using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDD.Core.Application.EventStoreModels
{
    public class GameModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [ConcurrencyCheck]
        public int Version { get; set; }
        public ICollection<EventModel> Events { get; set; }
    }
}
