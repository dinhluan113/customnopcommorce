﻿namespace Nop.Plugin.Api.Constants
{
    public class Configurations
    {
        // time is in minutes (30 days = 43200 minutes)
        public const int AccessTokenExpirationMinutes = 43200;
        public const int RefreshTokenExpirationMinutes = int.MaxValue;
        public const int DefaultLimit = 50;
        public const int DefaultPageValue = 1;
        public const int DefaultSinceId = 0;
        public const int DefaultCustomerId = 0;
        public const string DefaultOrder = "Id";
        public const int MaxLimit = 250;
        public const int MinLimit = 1;
        public const string PublishedStatus = "published";
        public const string UnpublishedStatus = "unpublished";
        public const string AnyStatus = "any";
        public const string JsonTypeMapsPattern = "json.maps";

        public const string NEWSLETTER_SUBSCRIBERS_KEY = "Nop.api.newslettersubscribers";
    }
}