//----------------------------------
//
//  create by tool 
//  2015-01-19 19:52:40
//
//----------------------------------


using System;

class Commond
{
    public const int Login_Rounte = 20201;                          //登陆路由
    public const int Login_Rounte_back = 40201;                     //登陆路由返回
    public const int Login_Account = 20202;                         //登陆帐号
    public const int Login_Account_back = 40202;                    //登陆帐号返回
    public const int Login_Gate = 20203;                            //登陆网关
    public const int Login_Gate_back = 40203;                       //登陆网关返回

    public const int Login_reConnect = 20206;                       //重连登陆
    public const int Login_reConnect_back = 40206;                  //重连登陆

    public const int Create_Guest = 20210;                          //创建游客帐号
    public const int Create_Guest_back = 40210;                     //创建游客帐号返回
    public const int Bind_Account = 20208;                          //绑定帐号
    public const int Bind_Account_back = 40208;                     //绑定帐号返回

    public const int Request_Role = 24101;                          //请求角色信息
    public const int Request_Role_back = 44101;                     //请求角色信息返回
    public const int Create_Role = 24102;                           //创建角色
    public const int Create_Role_back = 44102;                      //创建角色返回
    public const int Role_Change_Point = 44107;                     //角色修改消耗点数据

    public const int Request_Statistic_Data = 20800;                //请求角色的统计数据
    public const int Request_Statistic_Data_back = 40800;           //请求角色的统计数据返回

    public const int Request_Proficency_Data = 23300;               //请求角色的熟练赌
    public const int Request_Proficency_Data_back = 43300;          //请求角色的熟练度返回    

    public const int Request_Medel_Data = 20900;                    //请求角色勋章信息
    public const int Request_Medel_Data_back = 40900;               //请求角色勋章信息返回

    public const int Request_Online = 20212;                        //请求玩家在线信息
    public const int Request_Online_back = 40212;                   //请求玩家在线信息返回


    public const int Request_KitBag = 20501;                        //靖求背包
    public const int Request_KitBag_back = 40500;                   //靖求背包返回
    public const int Request_equiped_back = 40501;                  //已装备琥器返回

    public const int Request_putonAccessory = 20510;                //装配配件
    public const int Request_putonAccessory_back = 40510;           //装配配件返回

    public const int Request_Init_mail = 21201;                     //初始化mail
    public const int Request_Init_mail_back = 41201;                //初始化mail返回
    public const int Request_Read_mail = 21202;                     //阅读mail
    public const int Request_Read_mail_back = 41202;                //阅读mail返回
    public const int Request_Extract_mail = 21203;                  //提取附件
    public const int Request_Extract_mail_back = 41203;             //提取附件返回
    public const int Request_Delete_mail = 21204;                   //删除mail
    public const int Request_Delete_mail_back = 41204;              //删除mail返回
    public const int Mail_Receive_Add = 41205;                      //新增邮件

    public const int Request_Store = 25000;                         //请求商城
    public const int Request_Store_back = 45000;                    //请求商城返回
    public const int Request_Store_buy = 25001;                     //请求购买
    public const int Request_Store_buy_back = 45001;                //请求购买返回

    public const int Load_Exchange_RealPrize = 21701;               //请求实物兑换列表
    public const int Load_Exchange_RealPrize_back = 41701;          //请求实物兑换列表返回
    public const int Buy_RealPrize = 21702;                         //兑换实物
    public const int Buy_RealPrize_back = 41702;                    //兑换实物返回
    public const int Load_RealPrize_Record = 21703;                 //加载兑换记录
    public const int Load_RealPrize_Record_back = 41703;            //加载兑换记录返回

	public const int Request_Trade_Info = 25100;                    //请求交易列表
    public const int Request_Trade_Info_back = 45100;               //请求交易列表返回
    public const int Trade_Buy_Good = 25101;                        //购买交易的物品
    public const int Trade_Buy_Good_back = 45101;                   //购买交易的物品返回
    public const int Shelve_Good = 25102;                           //物品上架
    public const int Shelve_Good_back = 45102;                      //物品上架返回
    public const int Off_Shelve_Good = 25103;                       //物品下架
    public const int Off_Shelve_Good_back = 45103;                  //物品下架返回
    public const int Request_My_Trade_Info = 25104;                 //请求自己的售买消息
    public const int Request_My_Trade_Info_back = 45104;            //请求自己的售卖消息返回
    
	public const int Request_Task_Info = 23201;                     //向服务器请求任务信息,比如剩余任务次数
    public const int Request_Task_Info_back = 43201;                //向服务器请求任务信息,比如剩余任务次数
    public const int Request_Start_Task = 23202;                    //向服务器发送请求开始任务
    public const int Request_Start_Task_back = 43202;               //向服务器发送请求开始任务,回应
    public const int Request_Get_Task_Reward = 23203;               //向服务器发送请求任务奖励
    public const int Request_Get_Task_Reward_back = 43203;          //向服务器发送请求任务奖励,回应

    public const int Request_Send_Chat = 20701;                     //聊天
    public const int Request_Send_Chat_back = 40701;                //聊天返回,服务器端有消息过来也用这个消息id  // 0发送成功, 1.没有找到玩家, 2.发送内容过长  其中0还表示其他人发消息过来的通知
    public const int Request_Join_Channel = 20702;                  //加入聊天频道
    public const int Request_Join_Channel_back = 40702;             //加入聊天频道返回                       /* 0成功加入频道,  1:频道人数已满  2. 没有找到频道 3.你已经在该频道中*/
    public const int Request_Leave_Channel = 20703;                 //离开聊天频道
    public const int Request_Leave_Channel_back = 40703;            //离开聊天频道返回
    public const int Request_Load_Channel = 20704;                  //获得频道列表
    public const int Request_Load_Channel_back = 40704;             //获得频道列表返回

    public const int Request_Friend_list = 23001;                   //请求好友列表
    public const int Request_Friend_list_back = 43001;              //请求好友列表返回
    public const int Request_Add_Friend = 23002;                    //请求添加好友
    public const int Request_Add_Friend_back = 43002;               //请求添加好友返回
    public const int Request_Remove_Friend = 23003;                 //请求删除好友
    public const int Request_Remove_Friend_back = 43003;            //请求删除好友返回

    public const int Get_Info_Error = 44103;                        //加载信息错误

    public const int Request_RealPrize_Notice = 21704;              //请求兑换公告
    public const int Request_RealPrize_Notice_back = 41704;         //请求兑换公告返回   

    public const int MCC_Request_pay = 21501;                       //充值
    public const int MCC_Request_pay_back = 41501;                  //充值返回

    public const int Pay_WX_Mobile_back = 41512;                    //微信充值返回
    public const int Pay_ALi_Mobile_back = 41514;
    public const int Pay_WX_Mobile_Arrived_back = 41502;            //到账返回

    public const int Request_Play_Slots = 21801;                    //老虎机下注请求
    public const int Request_Play_Slots_back = 41801;               //老虎机下注返回

}