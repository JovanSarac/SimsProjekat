﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ceTe.DynamicPDF.Forms;
using ProjectSims.Domain.Model;
using ProjectSims.Domain.RepositoryInterface;
using ProjectSims.Observer;
using ProjectSims.Repository;

namespace ProjectSims.Service
{
    public class ForumService
    {
        private IForumRepository forumRepository;
        private IForumCommentRepository commentRepository;
        private ILocationRepository locationRepository;
        private IAccommodationRepository accommodationRepository;
        private IGuest1Repository guest1Repository;

        public ForumService()
        {
            forumRepository = Injector.CreateInstance<IForumRepository>();
            commentRepository = Injector.CreateInstance<IForumCommentRepository>();
            locationRepository = Injector.CreateInstance<ILocationRepository>();
            guest1Repository = Injector.CreateInstance<IGuest1Repository>();
            accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();


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

        public List<Forum> GetAllForumsByOwner(Owner owner)
        {
            List<Forum> forums = new List<Forum>();
            foreach (var id in owner.AccommodationIds)
            {
                Accommodation accommodation = accommodationRepository.GetById(id);
                foreach (var forum in GetAllForums())
                {
                    if (forum.LocationId == accommodation.IdLocation && forum.Status == ForumStatus.Otvoren)
                    {
                        forums.Add(forum);
                    }
                }
            }
            return forums;
        }
        public void CreateForum(Guest1 guest, Location location, string comment)
        {
            int id = forumRepository.NextId();
            Forum forum = new Forum(id, location.Id, location, comment, ForumStatus.Otvoren, guest.Id, guest, false);
            forumRepository.Create(forum);
        }

        public void CloseForum(int forumId)
        {
            Forum forumToClose = forumRepository.GetById(forumId);
            forumToClose.Status = ForumStatus.Zatvoren;
            forumRepository.Update(forumToClose);
        }

        public bool CheckIfVeryUseful(Forum forum)
        {
            return IsVeryUsefulForum(commentRepository.GetAllByForumId(forum.Id));
        }

        public bool IsVeryUsefulForum(List<ForumComment> comments)
        {
            int guestCommentsCounter = 0;
            int ownerCommentsCounter = 0;
            foreach (ForumComment comment in comments)
            {
                if (comment.IsGuest && comment.GuestVisited)
                {
                    guestCommentsCounter++;
                }
                else
                {
                    ownerCommentsCounter++;
                }
            }
            return guestCommentsCounter >= 20 && ownerCommentsCounter >= 10;
        }

        public void SetUsefulnessForEachForum(Owner owner)
        { 
            foreach (var forum in GetAllForumsByOwner(owner))
            {
                forum.IsVeryUseful = CheckIfVeryUseful(forum);
            }
        }
        public void Subscribe(IObserver observer)
        {
            forumRepository.Subscribe(observer);
        }
    }
}
