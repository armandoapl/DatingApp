using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            
            if (likesParams.Predicate == "liked")// the users that the currently login user has liked
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);// so here you use where to filter and returns the likeUser filters but all its props.
                users = likes.Select(like => like.LikedUser);// instead select of linQ is for select a spesific field of the table in this case the LikedUser (of type AppUser) prop.
            }


            if(likesParams.Predicate == "likedBy")// the users that has liked the currently login user
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUser = users.Select(user => 
                new LikeDto {
                    Id = user.Id,
                    UserName = user.UserName,
                    Age = user.DateOfBirth.CalculateAge(),
                    KnownAs = user.KnownAs,
                    PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                    City = user.City

                } 
            );

            return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)// this method here is to get a AppUser instance but with the LikedUsers.
        {
            return await _context.Users
                        .Include(x => x.LikedUsers)
                        .FirstAsync(x => x.Id == userId);
        }
    }
}
