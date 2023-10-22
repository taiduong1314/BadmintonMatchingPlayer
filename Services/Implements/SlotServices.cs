using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class SlotServices : ISlotServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public SlotServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public List<int> GetAvailable(int post_id, int num_slot)
        {
            var post = _repositoryManager.Post
                .FindByCondition(x => x.Id == post_id && x.QuantitySlot - x.Slots.Count() >= num_slot, true)
                .Include(x => x.Slots)
                .FirstOrDefault();
            if(post != null)
            {
                var res = new List<int>();
                for(var i = 0; i < num_slot; i++)
                {
                    post.Slots.Add(new Entities.Models.Slot
                    {

                    });
                }
                return res;
            }
            else
            {
                return new List<int>();
            }
        }
    }
}
