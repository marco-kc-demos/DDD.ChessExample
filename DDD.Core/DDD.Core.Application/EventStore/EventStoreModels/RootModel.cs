using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDD.Core.Application.EventStoreModels
{
    public class RootModel<TId>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TId Id { get; set; }
        [ConcurrencyCheck]
        public int Version { get; set; }
        public ICollection<EventModel> Events { get; set; }
    }
}
