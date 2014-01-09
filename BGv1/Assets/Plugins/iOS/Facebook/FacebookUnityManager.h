//
//  FacebookUnityManager.h
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 1/8/13.
//
//

#import <Foundation/Foundation.h>
#import "FacebookSDK.h"

#define UNITY_FBLISTENER_GAMEOBJECT_NAME        "FacebookListener"
#define UNITY_FBLISTENER_ONREQUEST_STARTED      "OnFBRequestStarted"
#define UNITY_FBLISTENER_ONSESSIONSTATECHANGED  "OnFbSessionStateChanged"
#define UNITY_FBLISTENER_ONPOSTTOWALL           "OnFbPostToWallResponded"
#define UNITY_FBLISTENER_ONGRAPHRESPONSE        "OnFbGraphRequestResponded"

@interface FacebookUnityManager : NSObject

@property (nonatomic, retain) FBSession * session;
@property (nonatomic, retain) NSDictionary * userInfo;
@property (nonatomic, retain) NSArray * userFriends;

@property (nonatomic, retain) FBRequestConnection *requestConnection;
@property (nonatomic, retain) UIViewController * overlayViewController;

+(FacebookUnityManager*)sharedManager;

-(FacebookUnityManager*)init;

-(void)openSession:(NSString*)fbAppId;

-(void)openSession:(NSString*)fbAppId completionBlock: (void (^)(BOOL)) callback;

-(void) facebookReconnect:(NSString*)fbAppId completionBlock: (void (^)(BOOL)) callback;

-(void)closeSession;

-(BOOL)hasOpenSession;

-(void)getUserInfoWithCompletionBlock:(void(^)(BOOL))callback;

-(NSString*)getUserId;

-(NSString*)getUserName;

-(void)getUserFriendsWithCompletionBlock:(void(^)(BOOL))callback;

-(NSInteger)getUserFriendsCount;

-(NSString*)getUserFriendIdAtIndex:(NSInteger)index;

-(NSString*)getUserFriendNameAtIndex:(NSInteger)index;

-(void)postUserFeed:(NSDictionary*)params completionBlock:(void(^)(BOOL)) callback;

-(void)postStatusUpdate:(NSString *) message;

-(void) uploadPhotoWithImage:(UIImage *) image;

-(void) uploadPhotoWithImage:(UIImage *) image completionBlock: (void (^)(NSString*)) callback;

-(void) initializeRequestConnection;

-(void) startShareRequestWithPhoto:(UIImage *) image parameters:(NSMutableDictionary *) params completionBlock:(void (^)(BOOL)) callback;

-(void) requestGraphPath:(NSString*)path;

-(void) requestGraphPath:(NSString*)path parameters:(NSDictionary*)params HTTPMethod:(NSString*)method completionBlock: (void (^)(BOOL)) callback;

-(void) requestWithGraphPath:(NSString*)path parameters:(NSDictionary*)params HTTPMethod:(NSString*)method completionBlock:(void (^)(BOOL)) callback;

-(void) presentWebDialogRequestModallyWithMessage:(NSString *) message title:(NSString *) title parameters:(NSDictionary *) parameters
                                         callback:(void(^)(BOOL)) callback;

@end
