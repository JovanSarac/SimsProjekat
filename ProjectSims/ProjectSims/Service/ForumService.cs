﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Domain.Model;
using ProjectSims.Domain.RepositoryInterface;

namespace ProjectSims.Service
{
    public class ForumService
    {
        private IForumRepository forumRepository;
        private ILocationRepository locationRepository;
        private IGuest1Repository guest1Repository;

        public ForumService()
        {
            forumRepository = Injector.CreateInstance<IForumRepository>();
            locationRepository = Injector.CreateInstance<ILocationRepository>();
            guest1Repository = Injector.CreateInstance<IGuest1Repository>();

            InitializeLocation();
            InitializeGuest();
        }

        private void InitializeLocation()
        {
            foreach (var forum in forumRepository.GetAll())
            {
                forum.Location = locationRepository.GetById(forum.LocationId);
            }
        }
        private void InitializeGuest()
        {
            foreach (var forum in forumRepository.GetAll())
            {
                forum.Guest = guest1Repository.GetById(forum.GuestId);
            }
        }

        public List<Forum> GetAllForums()
        {
            return forumRepository.GetAll();
        }
        public List<Forum> GetAllForumsByGuest(int guestId)
        {
            return forumRepository.GetAllByGuest(guestId);
        }

        public void CreateRating(Guest1 guest, Location location, string comment)
        {
            int id = forumRepository.NextId();
            Forum forum = new Forum(id, location.Id, location, comment, ForumStatus.Otvoren, guest.Id, guest);
            forumRepository.Create(forum);
        }

    }
}