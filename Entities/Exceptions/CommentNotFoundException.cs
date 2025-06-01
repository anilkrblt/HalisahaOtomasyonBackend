using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException(int commentId)
         : base($"Comment with id: {commentId} doesn't exist in the database.")
        {
        }
    }
}