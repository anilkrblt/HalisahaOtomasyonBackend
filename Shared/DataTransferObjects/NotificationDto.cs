using System;
using System.Text.Json.Serialization;
using Entities.Models;

namespace Shared.DataTransferObjects
{
    /*────────────  Okuma DTO’su  ────────────*/
    public class NotificationDto
    {
        public int      Id          { get; set; }
        public string?  Title       { get; set; }
        public string?  Content     { get; set; }
        public DateTime CreatedAt   { get; set; }
        public bool     IsRead      { get; set; }

        /* Hedef */
        public int?     UserId      { get; set; }
        public int?     FacilityId  { get; set; }

        /* Tür & İlişkilendirme */
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType Type { get; set; }

        /// <summary>İlgili kaydın (rezervasyon, yorum, vs.) Id’si</summary>
        public int?     RelatedId    { get; set; }

        /// <summary>"reservation", "comment" gibi serbest metin</summary>
        public string?  RelatedType  { get; set; }
    }

    /*────────────  Oluşturma DTO’su  ────────────*/
    public class NotificationForCreationDto
    {
        public string   Title        { get; set; } = null!;
        public string?  Content      { get; set; }

        /* Hedef */
        public int?     UserId       { get; set; }
        public int?     FacilityId   { get; set; }

        /* Tür & İlişkilendirme */
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType Type { get; set; } = NotificationType.Info;

        public int?     RelatedId    { get; set; }
        public string?  RelatedType  { get; set; }
    }
}
