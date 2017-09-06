//******************************************************************
// File Name:					EventID.cs
// Description:
// Author:						wuwei
// Date:						2016.12.29
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace FW.Event
{

    public enum EventID
    {
        //网络
        NET_CONN_STATE,                                 // 连接状态改变 (EventArg(NetMgr.CONN_MSG))
        NET_CONNSVR_START,                              // 开始连接服务器
        NET_CONNSVR_SUCCESS,                            // 连接服务器成功
        NET_CONNSVR_FAIL,                               // 连接服务器失败
        NET_RECONN_START,                               // 重连开始
        NET_RECONN_SUCCESS,                             // 重连成功
        NET_RECONN_FAIL,                                // 重连失败

        LOGIN_INFO,                                     //登陆消息（参数：EventArg(int 状态(0成功 1用户名 2密码错误 3gate验证错误))）
        Bind_Account,                                   //绑定帐号消息（参数：EventArg(int 状态(0：成功 1：失败2：账号名错误 3：旧密码错误4：新账号名格式不对5：新账号名 已被使用6：新密码格式不对7：手机号码格式不对))）

        Get_Info_Error,                                 //加载信息错误(参数：EventArg(int 错误码(1:加载角色基本数据,2:创建角色,3:加载背包,4:加载技能,5:加载任务,6:加载统计数据,
                                                                                              //7:加载邮件,8:加载商店,9:加载好友,10:加载关卡,11:加载实物仓库数据,12:加载实物兑换记录,
                                                                                              //13:加载挂机任务,14:加载交易信息))

        //role
        Role_inited,                                    //角色初始化完成
        WeaponList_changed,                             //武器表修改
        AccessoryList_changed,                          //配件表修改
        CommdityList_changed,                           //道具列表修改
        Weapon_AccessoryChanged,                        //武器配件装卸修改(参数：EventArg(WeaponBase 武器对象，offid 卸下配件id， onid 装配配件id))
        Weapon_PCEquipedChanged,                        //武器是否改变被装备信息(参数：EventArg(WeaponBase 武器对象，bool 是否被装配))
        Role_Change_Cash,                               //角色现金变化(参数：EventArg(Role 角色，int 数量))
        Role_Change_Diamond,                            //角色钻石变化(参数：EventArg(Role 角色，int 数量))
        Role_Change_Gold,                               //角色金币变化(参数：EventArg(Role 角色，int 数量))
        Role_Change_Exp,                                //角色经验变化(参数：EventArg(Role 角色，int 当前值，int 最大值))
        Role_Change_Level,                              //角色等级变化(参数：EventArg(Role 角色，int 等级))
        Role_Change_Point,                              //角色天赋点变化(参数：EventArg(Role 角色，int 天赋))
        Role_online_Change,                             //角色在线状态变化(参数：EventArg(Role 角色，int 1: 仅PC在线, 2: 仅手机在线, 3:PC和手机同时在线))

        //mail
        Mail_inited,                                    //邮件初始化完成
        Mail_readed,                                    //邮件阅读完成(参数：EventArg(Mail 邮件对象, bool 0成功1未成功))
        Mail_extracted,                                 //邮件提取附件完成(参数：EventArg(Mail 邮件对象, int 0成功1未成功2该邮件已过时))
        Mail_delete,                                    //邮件删除完成(参数：EventArg(Mail 邮件对象, int 0成功1未成功))
        Mail_Receive_New,                               //收到新邮件

        //friend
        Friend_inited,                                  //好友列表初始化
        Friend_add,                                     //添加好友 (参数：EventArg(GamePlayer 玩家对象,   0 成功   1 找不到该玩家 2该玩家已经是好友了  3好友数量满了))
        Friend_delete,                                  //删除好友 (参数：EventArg(GamePlayer 玩家对象,   0 成功   1 失败))

        //shop
        Shop_changed,                                   //商场刷新
        Shop_itemBought,                                //商品购买反馈(参数：EventArg(StoreItem 商品对象, bool 0成功1未成功))
        Shop_black_CountZero,                           //商品刷新倒计时

        //Exchange
        ExchnagePrizeItem_change,                        //兑换列表刷新
        ExchnagePrizeOrder_change,                       //订单列表刷新
        ExchangePrize_itemBought,                        //兑换商品  （参数：ExchangeItemOrder  兑换物品的订单   0.成功， 1.没有找到物品 2.库存不足 3.余额不足 ）

        //Deal
        Deal_itemBought,                                 //购买交易的物品  //0  成功   1 找不到玩家  2 类型错误  3 找不到物品  4 已售卖   15 你已经预购过了 
                                                         //5 交易不匹配   6发送邮件   7 创建邮件错误 8 钱不够
        Deal_putShelveItem,                              //上架返回 0 
        Deal_offShelveItem,                              //下架返回 0 成  1  失败
        Deal_itemChanged,                                //请求交易列表刷新
        Deal_mySelfItemChanged,                          //我的交易列表刷新

        //lOGIN UI
        UI_LOGIN_LOGIN_BUTTON,                                //登陆按钮点击
        UI_LOGIN_AUTOLOGIN_BUTTON,                            //自动登陆
        UI_LOGIN_QUICKLOGIN_BUTTON,                           //快速登陆
        UI_LOGIN_CHANGE_CONTENT,                              //内容变化（bool (false true  改变了)    int    0:账号   1:密码）

        //Main UI  主界面7个按钮，好友，邮件
        MAIN_UI_PLAYER_BTN,
        MAIN_UI_GAME_BTN,
        MAIN_UI_BAG_BTN,
        MAIN_UI_SHOP_BTN,
        MAIN_UI_EXCHANGE_BTN,
        MAIN_UI_RANK_BTN,
        MAIN_UI_CONVERTOOFFICAL_BTN,
        MAIN_UI_FIREND_BTN,
        MAIN_UI_EMAIL_BTN,
        MAIN_UI_DEAL_BTN,
        MAIN_UI_CLOSEDIALOGONE_BTN,
        MAIN_UI_Setting_BTN,
        MAIN_UI_LuckJoy_BTN,                                    //摇奖        


        MAIN_UI_REFRESHMAINUI,                                  //回到主界面调用



        //转为正式账号panel
        PANEL_CONVEROFFICAL_CONFIMR_BTN,                        //确定
        PANEL_CONVEROFFICAL_CANEL_BTN,                          //取消
        PANEL_CONVEROFFICAL_AUTOLOGIN_BTN,                      //自动登陆记忆

        //test  返回主界面
        PANEL_BACK_TO_MAIN_PANEL_BTN,

        Enter_ExchangePanel,                                    //由公告进入兑换


        //好友操作panel
        PANEL_FRIEND_ADDFRIEND_BTN,                             //加好友按钮
        PANEL_FRIEND_CONFIRM_ADDFRIEND_BTN,                     //确定添加好友按钮   
        PANEL_FRIEND_CANCEL_ADDFRIEND_BTN,                      //取消添加好友按钮
        PANEL_FRIEND_DELETEFRIEND_BTN,                          //删除好友按钮
        PANEL_FRIEND_CONFIRM_DELETEFRIEND_BTN,                  //确定删除好友按钮
        PANEL_FRIEND_CANCEL_DELETEFRIEND_BTN,                   //取消删除好友按钮
        PANEL_FRIEND_SEND_MSG_BTN,                              //发送消息的按钮
        //添加好友的事件info
        PANEL_FRIEND_ADDFRIEND_INFO,
        //删除好友的事件info
        PANEL_FRIEND_DELETEFRIEND_INFO,




        //Game 
        GAME_UI_BACK_TO_MAIN,                                    //返回主界面
        GAME_UI_ChatInputWordSend,                               //聊天的消息发送

        GAME_TaskInfo,                                           //任务数据刷新(int 1:正在挂机 2:没有挂机)
        GAME_TaskStart,                                            //开始一个任务
        GAME_TaskEnd,                                               //结束一个任务
        GAME_ScenePause,                                               //先把任务暂停
        GAME_SecondInvoke,                                              //每一秒触发,返回结束时间     (int 任务剩余时间)
        
        GAME_MoveStart,                                         //移动开始
        GAME_EnemyDie,                                          //敌人死亡
        GAME_GameOver,                                        //游戏失败
        GAME_GameWin,                                        //游戏胜利

        //Chat
        Chat_ReceiveInfoNotify,                                //收到消息提示
        Chat_SendChatError,                                    //发送消息失败
        Chat_JoinChannelBack,                                   //加入频道反馈
        //角色界面的 子页面  战绩界面

        PLAYER_RECORD_INFO,                                      //战绩请求
        PLAYER_MEDEL_INFO,                                       //勋章
        PLAYER_ROLE_INFO,                                        //形象
        PLAYER_WEAPON_INFO,                                      //武器

        //微信回调
        WChat_back_Info,                                         //微信回调

        //充值返回
        MCC_Pay_Back_Info,                                       //充值返回/* 0 请求成功, 1. 充值id错误, 2.渠道id错误*/                                        
        MCC_Pay_Arraival,                                        //到账

        //摇奖
        Refresh_histroy,                                         //刷新摇奖记录
        LuckyJoy_Begin,                                          //开启摇奖
        LuckyJoy_Fail,                                           //开启摇奖失败   
    }
}