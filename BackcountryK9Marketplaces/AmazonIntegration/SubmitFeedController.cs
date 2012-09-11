using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketplaceWebService.Model;
using System.IO;
using MarketplaceWebService;

namespace BackcountryK9Marketplaces.AmazonIntegration
{
    /// <summary>
    /// Class that contains all of the logic for making submit feed calls to the amazon web service.
    /// </summary>
    public class SubmitFeedController
    {
        /// <summary>
        /// The local instance of the amazon client library service client.
        /// </summary>
        private MarketplaceWebServiceClient _AmazonClient;
        private string _MarketplaceId;
        private string _MerchantId;

        public SubmitFeedController(MarketplaceWebServiceClient amazonClient, string merchantId, string marketplaceId)
        {
            _AmazonClient = amazonClient;
            _MerchantId = merchantId;
            _MarketplaceId = marketplaceId;
        }

        public Stream SubmitFeedAndGetResponse(Stream dataStream, string feedType)
        {
            var submitFeedRequest = CreateSubmitFeedRequest(dataStream, feedType);
            SubmitFeedResponse submitFeedResponse = null;
            try
            {
                submitFeedResponse = _AmazonClient.SubmitFeed(submitFeedRequest);
            }
            catch (MarketplaceWebServiceException ex)
            {
                // Make sure the file stream gets closed after everything.
                dataStream.Close();
                throw ex;
            }
            // Make sure the file stream gets closed after everything.
            dataStream.Close();

            // If the previous errors out then don't run this. Return an exception.
            WaitForGetFeedSubmissionListToComplete(submitFeedResponse.SubmitFeedResult.FeedSubmissionInfo.FeedSubmissionId);

            return GetFeedSubmissionResultStream(submitFeedResponse.SubmitFeedResult.FeedSubmissionInfo.FeedSubmissionId);
        }

        /// <summary>
        /// Creates the SubmitFeedRequest given a stream of content
        /// </summary>
        /// <param name="dataContent">The stream of the content</param>
        /// <param name="feedType">The type of feed to be submitted.</param>
        /// <returns>A SubmitFeedRequest object</returns>
        private SubmitFeedRequest CreateSubmitFeedRequest(Stream dataContent, string feedType)
        {
            var request = new SubmitFeedRequest();
            request.Merchant = _MerchantId;
            request.MarketplaceIdList = new IdList();
            request.MarketplaceIdList.Id = new List<string>(new string[] { _MarketplaceId });
            request.FeedContent = dataContent;
            // Set the MD5 hash of the content
            request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
            // Reset the position of the reader of the FeedContent
            request.FeedType = feedType;
            return request;
        }

        /// <summary>
        /// Will continue to call GetFeedSubmissionList until it's completed or canceled
        /// </summary>
        /// <param name="feedSubmissionId">The feed submission id.</param>
        private void WaitForGetFeedSubmissionListToComplete(string feedSubmissionId)
        {
            GetFeedSubmissionListRequest submissionListRequest = new GetFeedSubmissionListRequest();
            submissionListRequest.Merchant = _MerchantId;
            submissionListRequest.FeedSubmissionIdList = new IdList() { Id = { feedSubmissionId } };
            GetFeedSubmissionListResponse submissionListResponse = null;
            // Pause for 5 seconds to give Amazon a little bit to try and process. Otherwise we have to wait 45 seconds.
            System.Threading.Thread.Sleep(5000);
            do
            {
                // Check to see if it's the first time it's been called.
                if (submissionListResponse != null)
                    // If it's not finished yet, sleep for 45 seconds. This is the restore rate for GetFeedSubmissionList
                    System.Threading.Thread.Sleep(45000);
                submissionListResponse = _AmazonClient.GetFeedSubmissionList(submissionListRequest);
            }
            while (!submissionListResponse.GetFeedSubmissionListResult.FeedSubmissionInfo.First().FeedProcessingStatus.Equals("_CANCELED_")
                && !submissionListResponse.GetFeedSubmissionListResult.FeedSubmissionInfo.First().FeedProcessingStatus.Equals("_DONE_"));

        }

        /// <summary>
        /// Handles calling the GetFeedSubmissionResult method on Amazon's webservice and returns the stream
        /// </summary>
        /// <param name="feedSubmissionId">The id of the feed submission</param>
        /// <returns>A stream of the xml result</returns>
        private Stream GetFeedSubmissionResultStream(string feedSubmissionId)
        {
            GetFeedSubmissionResultRequest feedSubmissionResultRequest = new GetFeedSubmissionResultRequest();
            feedSubmissionResultRequest.Merchant = _MerchantId;

            feedSubmissionResultRequest.FeedSubmissionId = feedSubmissionId;
            var stream = new MemoryStream();
            feedSubmissionResultRequest.FeedSubmissionResult = stream;

            _AmazonClient.GetFeedSubmissionResult(feedSubmissionResultRequest);
            return feedSubmissionResultRequest.FeedSubmissionResult;
        }
    }
}
