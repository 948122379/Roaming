using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRecorder{
    public struct RoomNeighbor
    {
        public int selfroom;
        public int uproom;
        //public GameObject downroom;
        public int leftroom;
        public int rightroom;
        public int forwardroom;
        public int backroom;
        public float scale;
        public Vector3 roomSize;
        public Vector3 position;
    }
    public float[] WallRotation = new float[]{180f,0f,90f,-90f,90f,-90f};
    public struct WallsOfRoom
    {
        public int selfroom;
        public Vector3 forwardwallOffset;
        public Vector3 backwallOffset;
        public Vector3 leftwallOffset;
        public Vector3 rightwallOffset;
        public Vector3 upwallOffset;
        public Vector3 downwallOffset;
    }
    private Vector3 minRoomSize = new Vector3(0.94f, 0.94f, 1.04f);//适配最小长宽高
    private Vector3 maxRoomSize = new Vector3(11.3f, 11.3f, 5.2f);//适配最大长宽高
    private Vector3 originSize = new Vector3(11.3f,5.2f,4.7f);//原始的长高宽
    private float interval = 0f;//房间间隔
    private float distanceFloat = 0.4f;//房间位置浮动
    public float minsize = 0.2f;//最小Scale
    public float maxsize = 1f;//最大Scale
    private float mirrorWallInterval = 0.2f;//镜子与墙的间隔
    public RoomNeighbor[] rooms;
    public WallsOfRoom[] walls;
	public void Init () {
        Reorder();
	}
    void Reorder()
    {
        GameObject[] startroomselfs=GameObject.FindGameObjectsWithTag("Room");
        GameObject[] roomselfs = new GameObject[startroomselfs.Length];
        rooms = new RoomNeighbor[roomselfs.Length];
        for(int i=0;i<startroomselfs.Length;i++)
        {
            string[] namepart={"Room",i.ToString()};
            string name = string.Concat(namepart);
            roomselfs[i] = GameObject.Find(name);
            if (i == startroomselfs.Length-1)
            {
                GetRoomsSize(roomselfs);
            }
        }
        
    }
    void GetRoomsSize(GameObject[] roomselfs)
    {
        for (int i = 0; i < roomselfs.Length; i++)
        {
            float sizeSelf = maxsize;//获取自身比例
            rooms[i].scale = 1f;
            if (roomselfs[i].transform.localScale.x < 0.3f)
            {
                sizeSelf = minsize;
                rooms[i].scale = 0.2f;
            }

            rooms[i].roomSize = sizeSelf * originSize;
            int roomRotationY = 0;
            if (roomselfs[i].transform.eulerAngles.y > 0)
            {
                roomRotationY = Mathf.RoundToInt(roomselfs[i].transform.eulerAngles.y % 180);
            }
            else
            {
                roomRotationY = Mathf.RoundToInt((360 + roomselfs[i].transform.eulerAngles.y % 360) % 180);
            }
            if (roomRotationY > 88 && roomRotationY < 92)
            {
                rooms[i].roomSize = new Vector3(sizeSelf * originSize.z, sizeSelf * originSize.y, sizeSelf * originSize.x);
            }

            rooms[i].position = roomselfs[i].transform.position;

            if (i == roomselfs.Length - 1)
            {
                GetNeighborRoom(roomselfs);
            }
        }
    }
    void GetNeighborRoom(GameObject[] roomselfs) 
    {
        //获得最大可能的附近房子的范围
        float minNearDistance;
        float maxNearDistance;
        float maxDistanceMaybe;
        float maxDistanceMaybeXZ = Mathf.Sqrt(Mathf.Pow(originSize.x / 2 + originSize.z / 2, 2) + Mathf.Pow(originSize.x / 2 - originSize.z / 2, 2));
        float maxDistanceMaybeXY = Mathf.Sqrt(Mathf.Pow(originSize.x / 2 + originSize.y / 2, 2) + Mathf.Pow(originSize.x / 2 - originSize.y / 2, 2));
        float maxDistanceMaybeYZ = Mathf.Sqrt(Mathf.Pow(originSize.y / 2 + originSize.z / 2, 2) + Mathf.Pow(originSize.y / 2 - originSize.z / 2, 2));
        Vector3 minToMax;
        if (originSize.x > originSize.y)
        {
            if (originSize.z > originSize.x)
            {
                minToMax = new Vector3(originSize.y, originSize.x, originSize.z);
                maxNearDistance=originSize.z;
            }
            else if (originSize.z < originSize.y)
            {
                minToMax = new Vector3(originSize.z, originSize.y, originSize.x);
                maxNearDistance=originSize.x;
            }
            else
            {
                minToMax = new Vector3(originSize.y, originSize.z, originSize.x);
                maxNearDistance=originSize.x;
            }
        }
        else
        {
            if (originSize.z > originSize.y)
            {
                minToMax = new Vector3(originSize.x, originSize.y, originSize.z);
                maxNearDistance=originSize.z;
            }
            else if (originSize.z < originSize.x)
            {
                minToMax = new Vector3(originSize.z, originSize.x, originSize.y);
                maxNearDistance=originSize.x;
            }
            else
            {
                minToMax = new Vector3(originSize.x, originSize.z, originSize.y);
                maxNearDistance=originSize.y;
            }
        }
        //int yBelong = 0;
        if (maxDistanceMaybeXZ > maxDistanceMaybeXY && maxDistanceMaybeXZ > maxDistanceMaybeYZ)
        {
            maxDistanceMaybe = maxDistanceMaybeXZ;
        }
        else if (maxDistanceMaybeXY > maxDistanceMaybeXZ && maxDistanceMaybeXY > maxDistanceMaybeYZ)
        {
            maxDistanceMaybe = maxDistanceMaybeXY;
        }
        else{
            maxDistanceMaybe = maxDistanceMaybeYZ;
        }
        minNearDistance = minToMax.x* minsize;
        maxNearDistance = minToMax.z* maxsize;
        if (2 * maxNearDistance < maxDistanceMaybe)
        {
            maxNearDistance = maxDistanceMaybe;
        }

        //搜寻附近的房间
        int room_i = 0;
        foreach (GameObject room in roomselfs)//定位其他房间
        {
            //rooms[room_i].selfroom = int.Parse(room.name.Substring(5,room.name.Length));
            rooms[room_i].selfroom = room_i;
            rooms[room_i].leftroom = -1;
            rooms[room_i].rightroom = -1;
            rooms[room_i].forwardroom = -1;
            rooms[room_i].backroom = -1;
            rooms[room_i].uproom = -1;
            for(int i = 0; i < roomselfs.Length ; i++)
            {
                if (roomselfs[i] != room)
                {
                    if (Vector3.Distance(roomselfs[i].transform.position, room.transform.position) > minNearDistance + interval - distanceFloat
                        && Vector3.Distance(roomselfs[i].transform.position, room.transform.position) < maxNearDistance + interval + distanceFloat)
                    {
                        //是否互相平行
                        bool IsParallel = (roomselfs[i].transform.eulerAngles.y % 180 > room.transform.eulerAngles.y % 180 - 2 && roomselfs[i].transform.eulerAngles.y % 180 < room.transform.eulerAngles.y % 180 + 2);

                        //上下
                        bool IsNearHeight = Vector3.Distance(new Vector3(roomselfs[i].transform.position.x, 0, roomselfs[i].transform.position.z), new Vector3(room.transform.position.x, 0, room.transform.position.z)) <= maxDistanceMaybeXZ + distanceFloat;
                        if (IsParallel)
                        {
                            if (rooms[room_i].roomSize.x > rooms[room_i].roomSize.z)
                            {
                                IsNearHeight = IsNearHeight && (Mathf.Abs(roomselfs[i].transform.position.z - room.transform.position.z) <= distanceFloat && Mathf.Abs(roomselfs[i].transform.position.x - room.transform.position.x) <= (rooms[room_i].roomSize.x / 2 + rooms[i].roomSize.x / 2) - Mathf.Min(rooms[room_i].roomSize.z / 2, rooms[i].roomSize.z / 2) + distanceFloat);
                            }
                            else
                            {
                                IsNearHeight = IsNearHeight && (Mathf.Abs(roomselfs[i].transform.position.x - room.transform.position.x) <= distanceFloat && Mathf.Abs(roomselfs[i].transform.position.z - room.transform.position.z) <= (rooms[room_i].roomSize.z / 2 + rooms[i].roomSize.z / 2) - Mathf.Min(rooms[room_i].roomSize.x / 2, rooms[i].roomSize.x / 2) + distanceFloat);
                            }
                        }
                        else
                        {
                            IsNearHeight = IsNearHeight && (Mathf.Abs(roomselfs[i].transform.position.x - room.transform.position.x) < Mathf.Abs(rooms[room_i].roomSize.x - rooms[i].roomSize.x) / 2 + distanceFloat)
                                                        && (Mathf.Abs(roomselfs[i].transform.position.z - room.transform.position.z) < Mathf.Abs(rooms[room_i].roomSize.z - rooms[i].roomSize.z) / 2 + distanceFloat);
                        }
                        if (IsNearHeight)
                        {
                            if (roomselfs[i].transform.position.y > room.transform.position.y + rooms[room_i].roomSize.y / 2 - distanceFloat
                                && roomselfs[i].transform.position.y <= room.transform.position.y + (rooms[room_i].roomSize.y / 2 + rooms[i].roomSize.y / 2) + distanceFloat)//上方
                            {
                                if (rooms[room_i].uproom == -1 
                                    ||roomselfs[i].transform.position.y < roomselfs[rooms[room_i].uproom].transform.position.y)
                                {
                                    rooms[room_i].uproom = i;
                                    //Debug.Log(rooms[room_i].selfroom + " up " + i);
                                }
                            }
                            /*else//下方
                            {
                            }*/
                        }

                        //前后（Vector3.Dot>0前方，<0后方）
                        bool IsNearWeight = (Mathf.Abs(roomselfs[i].transform.position.y - room.transform.position.y) <= originSize.y/2 + distanceFloat)
                                            &&(Vector3.Distance(new Vector3(roomselfs[i].transform.position.x, roomselfs[i].transform.position.y, 0), new Vector3(room.transform.position.x, room.transform.position.y, 0)) <= maxDistanceMaybeXY + distanceFloat);
                        if (IsParallel)
                        {
                            if (rooms[room_i].roomSize.x > rooms[room_i].roomSize.z)
                            {
                                IsNearWeight = IsNearWeight && (Mathf.Abs(roomselfs[i].transform.position.x - room.transform.position.x) <= (rooms[room_i].roomSize.x / 2 + rooms[i].roomSize.x / 2) - Mathf.Min(rooms[room_i].roomSize.z / 2, rooms[i].roomSize.z / 2) + distanceFloat);
                            }
                            else
                            {
                                IsNearWeight = IsNearWeight && (Mathf.Abs(roomselfs[i].transform.position.x - room.transform.position.x) < distanceFloat);
                            }
                        }
                        else
                        {
                            IsNearWeight = IsNearWeight && (Mathf.Abs(roomselfs[i].transform.position.x - room.transform.position.x) < Mathf.Abs(rooms[room_i].roomSize.x - rooms[i].roomSize.x) / 2 + distanceFloat);
                        }
                        if (IsNearWeight)
                        {
                            if (roomselfs[i].transform.position.z > room.transform.position.z + rooms[room_i].roomSize.z / 2 - distanceFloat
                                && roomselfs[i].transform.position.z <= room.transform.position.z + (rooms[room_i].roomSize.z / 2 + rooms[i].roomSize.z / 2) + distanceFloat)//前
                            {
                                if (rooms[room_i].forwardroom == -1 
                                    || roomselfs[i].transform.position.z < roomselfs[rooms[room_i].forwardroom].transform.position.z)
                                {
                                    rooms[room_i].forwardroom = i;
                                    //Debug.Log(rooms[room_i].selfroom + " forward " + i);
                                }
                            }
                            else if (roomselfs[i].transform.position.z < room.transform.position.z - rooms[room_i].roomSize.z / 2 + distanceFloat
                                && roomselfs[i].transform.position.z >= room.transform.position.z - (rooms[room_i].roomSize.z / 2 + rooms[i].roomSize.z / 2) - distanceFloat)//后
                            {
                                if (rooms[room_i].backroom == -1
                                    || roomselfs[i].transform.position.z > roomselfs[rooms[room_i].backroom].transform.position.z)
                                {
                                    rooms[room_i].backroom = i;
                                    //Debug.Log(rooms[room_i].selfroom + " back " + i);
                                }
                            }
                        }

                        //左右
                        bool IsNearLong = (Mathf.Abs(roomselfs[i].transform.position.y - room.transform.position.y) <= originSize.y / 2 + distanceFloat)
                                            && Vector3.Distance(new Vector3(0, roomselfs[i].transform.position.y, roomselfs[i].transform.position.z), new Vector3(0, room.transform.position.y, room.transform.position.z)) <= maxDistanceMaybeYZ + distanceFloat;

                        if (IsParallel)
                        {
                            if (rooms[room_i].roomSize.x > rooms[room_i].roomSize.z)
                            {
                                IsNearLong = IsNearLong && (Mathf.Abs(roomselfs[i].transform.position.z - room.transform.position.z) < distanceFloat);
                            }
                            else
                            {
                                IsNearLong = IsNearLong && (Mathf.Abs(roomselfs[i].transform.position.z - room.transform.position.z) < (rooms[room_i].roomSize.z / 2 + rooms[i].roomSize.z / 2) - Mathf.Min(rooms[room_i].roomSize.x / 2, rooms[i].roomSize.x / 2) + distanceFloat);
                            }
                        }
                        else
                        {
                            IsNearLong = IsNearLong && (Mathf.Abs(roomselfs[i].transform.position.z - room.transform.position.z) <=  Mathf.Abs(rooms[room_i].roomSize.z - rooms[i].roomSize.z) / 2 + distanceFloat);
                        }
                        if (IsNearLong)
                        {
                            if (roomselfs[i].transform.position.x > room.transform.position.x + rooms[room_i].roomSize.x/2 - distanceFloat
                                && roomselfs[i].transform.position.x <= room.transform.position.x + (rooms[room_i].roomSize.x/2 + rooms[i].roomSize.x/2) + distanceFloat)//右
                            {
                                if (rooms[room_i].rightroom == -1
                                    || roomselfs[i].transform.position.x < roomselfs[rooms[room_i].rightroom].transform.position.x)
                                {
                                    rooms[room_i].rightroom = i;
                                    //Debug.Log(rooms[room_i].selfroom + " right " + i);
                                }
                            }
                            else if (roomselfs[i].transform.position.x < room.transform.position.x - rooms[room_i].roomSize.x / 2 + distanceFloat
                                && roomselfs[i].transform.position.x >= room.transform.position.x - (rooms[room_i].roomSize.x / 2 + rooms[i].roomSize.x / 2) - distanceFloat)//左
                            {
                                if (rooms[room_i].leftroom == -1
                                   || roomselfs[i].transform.position.x > roomselfs[rooms[room_i].leftroom].transform.position.x)
                                {
                                    rooms[room_i].leftroom = i;
                                    //Debug.Log(rooms[room_i].selfroom + " left " + i);
                                }
                            }
                        }
                    }
                }
            }
            room_i++;
        }
        GetWalls(roomselfs);
	}

    void GetWalls(GameObject[] roomselfs)
    {
        walls = new WallsOfRoom[roomselfs.Length];
        for (int i = 0; i < roomselfs.Length; i++)
        {
            walls[i].selfroom = i;
            //Vector3 modelsize = roomselfs[i].GetComponent<MeshFilter>().mesh.bounds.size;
            //float realysize = modelsize.x * roomselfs[i].transform.localScale.x;
            walls[i].leftwallOffset = roomselfs[i].transform.position + new Vector3(-rooms[i].roomSize.x / 2 + mirrorWallInterval * rooms[i].scale, 0f, 0f);
            walls[i].rightwallOffset = roomselfs[i].transform.position + new Vector3(rooms[i].roomSize.x / 2 - mirrorWallInterval * rooms[i].scale, 0f, 0f);
            walls[i].forwardwallOffset = roomselfs[i].transform.position + new Vector3(0f, 0f, rooms[i].roomSize.z / 2 - mirrorWallInterval * rooms[i].scale);
            walls[i].backwallOffset = roomselfs[i].transform.position + new Vector3(0f, 0f, -rooms[i].roomSize.z / 2 + mirrorWallInterval * rooms[i].scale);
            walls[i].upwallOffset = roomselfs[i].transform.position + new Vector3(0f, rooms[i].roomSize.y / 2 - mirrorWallInterval * rooms[i].scale, 0f);
            walls[i].downwallOffset = roomselfs[i].transform.position + new Vector3(0f, -rooms[i].roomSize.y / 2 + mirrorWallInterval * rooms[i].scale, 0f);
        }
        //把第8层的房间转过来
        GameObject.Find("Room8").transform.eulerAngles = new Vector3(90, 90, 0);
    }


    
}
