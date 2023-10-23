﻿using Entities.Models;
using Entities.RequestObject;
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

        public bool Discard(List<int> slotsId, int post_id)
        {
            var slots = _repositoryManager.Slot.FindByCondition(x => x.IdPost == post_id && slotsId.Contains(x.Id), true).ToList();
            if(slots.Count == slotsId.Count)
            {
                foreach(var slot in slots)
                {
                    slot.IsDeleted = true;
                }
                _repositoryManager.SaveAsync().Wait();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int> GetAvailable(CheckAvailableSlot info)
        {
            var slots = _repositoryManager.Slot
                .FindByCondition(x => x.IdPost == info.PostId && x.ContentSlot == info.DateRegis && !x.IsDeleted, true)
                .Include(x => x.IdPostNavigation)
                .ToList();
            if(slots.Count > 0)
            {
                if(slots[0].IdPostNavigation != null)
                {
                    var avaiSlot = slots[0].IdPostNavigation.QuantitySlot - info.NumSlot;
                    if (avaiSlot >= 0)
                    {
                        var res = new List<int>();
                        for(var i = 0; i <= info.NumSlot; i++)
                        {
                            var slot = new Slot
                            {
                                ContentSlot = info.DateRegis,
                                IdPost = info.PostId,
                                IdUser = info.UserId
                            };
                            _repositoryManager.Slot.Create(slot);
                            _repositoryManager.SaveAsync().Wait();
                            res.Add(slot.Id);
                        }
                        return res;
                    }
                }
            }
            return new List<int>();
        }
    }
}
