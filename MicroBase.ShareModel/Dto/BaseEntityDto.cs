using System;

namespace MicroBase.Share.Dto
{
    public abstract class BaseEntityDto
    {
        /// <summary>
        /// The Id of account trigger the action
        /// </summary>
        public Guid? ActionByAccountId { get;set; } 
    }
}