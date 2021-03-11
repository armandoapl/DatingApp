using API.DTO;
using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId); // go and get a specific UserLiked Instance
        Task<AppUser> GetUserWithLikes(int userId);// 
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams); // in here the predicate is for saying if we want users that has been liked by us, or the users that has liked us.

    }
}
