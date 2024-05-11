using Newtonsoft.Json;

namespace TiktokTools;

public class Message
{
    [JsonProperty("Type")] public int Type { get; set; }
    public class DataNode
    {
        [JsonProperty("MsgId")] public long MsgId { get; set; }
        public class UserNode
        {
            [JsonProperty("FollowingCount")] public int FollowingCount { get; set; }
            [JsonProperty("Id")] public long Id { get; set; }
            [JsonProperty("ShortId")] public long ShortId { get; set; }
            [JsonProperty("DisplayId")] public string DisplayId { get; set; }
            [JsonProperty("Nickname")] public string Nickname { get; set; }
            [JsonProperty("Level")] public int Level { get; set; }
            [JsonProperty("PayLevel")] public int PayLevel { get; set; }
            [JsonProperty("Gender")] public int Gender { get; set; }
            [JsonProperty("HeadImgUrl")] public string HeadImgUrl { get; set; }
            [JsonProperty("SecUid")] public string SecUid { get; set; }
            public class FansClubNode
            {
                [JsonProperty("ClubName")]
                public string ClubName { get; set; }

                [JsonProperty("Level")]
                public int Level { get; set; }
            }
            [JsonProperty("FansClub")] public FansClubNode FansClub { get; set; }
            [JsonProperty("FollowerCount")] public int FollowerCount { get; set; }
            [JsonProperty("FollowStatus")] public int FollowStatus { get; set; }
        }
        [JsonProperty("User")] public UserNode User { get; set; }
        [JsonProperty("Content")] public string Content { get; set; }
        [JsonProperty("RoomId")] public long RoomId { get; set; }
    }
    [JsonProperty("Data")] public DataNode Data { get; set; }

    public static Message FromJson(string jsonString)
    {
        return JsonConvert.DeserializeObject<Message>(jsonString);
    }
}

