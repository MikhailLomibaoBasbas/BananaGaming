//
//  FacebookUnityManager.m
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 1/8/13.
//
//

#import "FacebookUnityManager.h"

NSTimeInterval timeBeforeCancelling = 10.0;

@implementation FacebookUnityManager
@synthesize session;
@synthesize userInfo;
@synthesize userFriends;
@synthesize overlayViewController;
@synthesize requestConnection;

+(FacebookUnityManager*)sharedManager {
    static FacebookUnityManager * sharedInstance;
    if(sharedInstance == NULL)
        sharedInstance = [[FacebookUnityManager alloc] init];
    return sharedInstance;
}

#pragma mark Public Member Methods

-(FacebookUnityManager*)init {
    if(self = [super init]){
        FBSessionTokenCachingStrategy* pToken = [[[FBSessionTokenCachingStrategy alloc]initWithUserDefaultTokenInformationKeyName:@"STokenInfoX"]autorelease];
        session = [[FBSession alloc] initWithAppID:[[[NSBundle mainBundle] infoDictionary] objectForKey:@"FacebookAppId"]
                                                       permissions:[NSArray arrayWithObject:@"status_update"]
                                                   urlSchemeSuffix:@""
                                                tokenCacheStrategy: pToken];
        FBSession.activeSession = session;
    }
    return self;
}

-(void)openSession:(NSString*)fbAppId {
    [self openSession:fbAppId completionBlock:nil];
}

-(void) facebookReconnect:(NSString*)fbAppId completionBlock: (void (^)(BOOL)) callback {
    [FBSession.activeSession closeAndClearTokenInformation];
    [FBSession renewSystemCredentials:^(ACAccountCredentialRenewResult result,
                                        NSError *error)
     {
         [self openSession:fbAppId completionBlock:callback];
     }];
}

-(void)openSession:(NSString*)fbAppId completionBlock: (void (^)(BOOL)) callback {
    NSLog(@"%s", __func__);
    NSLog(@"Logging in to FB with FBAPPID: %@", fbAppId);
    [FBSettings setDefaultAppID:fbAppId];
    [FBSession openActiveSessionWithReadPermissions:nil allowLoginUI:YES completionHandler:
     ^(FBSession * session, FBSessionState state, NSError * error){
         if(error!=nil){
             NSLog(@"Failed logging in to FB: %@", error.localizedDescription);
             callback(false);
         }else{
             NSLog(@"FBSessionState: %d", state);
             switch (state) {
                 case FBSessionStateOpen:
                 case FBSessionStateOpenTokenExtended:
                     callback(true);
                     break;
                case FBSessionStateClosedLoginFailed:
                     callback(false);
                     break;
                 default:
                     break;
             }
         }
     }];
}

-(void)closeSession {
    NSLog(@"%s", __func__);
    [FBSession.activeSession closeAndClearTokenInformation];
}

-(BOOL)hasOpenSession {
    NSLog(@"%s", __func__);
    return FBSession.activeSession.isOpen;
}

-(void)getUserInfoWithCompletionBlock:(void(^)(BOOL))callback {
    __block FacebookUnityManager * selfPointer = self;
    [FBRequestConnection startWithGraphPath:@"me"
                                 parameters:nil
                                 HTTPMethod:@"GET"
                          completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
                              NSLog(@"[FacebookManager requestGraphPath] Result: %@ Error: %@", result, [error localizedDescription]);
                              selfPointer.userInfo = result;
                              callback(error==nil);
                          }];
}

-(NSString*)getUserId {
    return [userInfo objectForKey:@"id"];
}

-(NSString*)getUserName {
    return [userInfo objectForKey:@"name"];
}

-(void)getUserFriendsWithCompletionBlock:(void(^)(BOOL))callback {
    __block FacebookUnityManager * selfPointer = self;
    [FBRequestConnection startWithGraphPath:@"me/friends"
                                 parameters:nil
                                 HTTPMethod:@"GET"
                          completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
                              NSLog(@"[FacebookManager requestGraphPath] Result: %@ Error: %@", result, [error localizedDescription]);
                              selfPointer.userFriends = result[@"data"];
                              callback(error==nil);
                          }];
}

-(NSInteger)getUserFriendsCount {
    return [userFriends count];
}

-(NSString*)getUserFriendIdAtIndex:(NSInteger)index {
    NSDictionary * userFriend = [userFriends objectAtIndex:index];
    return [userFriend objectForKey:@"id"];
}

-(NSString*)getUserFriendNameAtIndex:(NSInteger)index {
    NSDictionary * userFriend = [userFriends objectAtIndex:index];
    return [userFriend objectForKey:@"name"];
}

-(void)postUserFeed:(NSDictionary*)params completionBlock:(void(^)(BOOL)) callback {
    NSLog(@"%s", __func__);
    [self performPublishAction:^{
        [FBRequestConnection startWithGraphPath:@"me/feed"
                                     parameters:params
                                     HTTPMethod:@"POST"
                              completionHandler:^(FBRequestConnection *connection, id result, NSError *error) {
                                  callback(error == nil);
                              }];
    }];
}

-(void)postStatusUpdate:(NSString *)message {
    NSLog(@"%s", __func__);
    [self createOverlayViewController];
    // if it is available to us, we will post using the native dialog
    /*BOOL displayedNativeDialog = [FBNativeDialogs presentShareDialogModallyFrom: self.overlayViewController
                                                                    initialText:message
                                                                          image:nil
                                                                            url:nil
                                                                        handler:
                                  ^(FBNativeDialogResult result, NSError *error){
                                      NSLog(@"[FacebookManager postStatusUpdate] Result: %d Error: %@", result, [error localizedDescription]);
                                      NSString * statusMessage = [NSString stringWithFormat:@"%d|%@", result, [error localizedDescription]];
                                      const char * statusMessageC = [statusMessage cStringUsingEncoding:NSStringEncodingConversionAllowLossy];
                                      UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONPOSTTOWALL, statusMessageC);
                                      [self destroyOverlayViewController];
                                  }];
    
    if (!displayedNativeDialog) {
        
        [self performPublishAction:^{
            // otherwise fall back on a request for permissions and a direct post
            [FBRequestConnection startForPostStatusUpdate:message
                                        completionHandler:^(FBRequestConnection *connection, id result, NSError *error) {
                                            NSLog(@"[FacebookManager postStatusUpdate] Result: %@ Error: %@", result, [error localizedDescription]);
                                            NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", result, [error localizedDescription]];
                                            const char * statusMessageC = [statusMessage cStringUsingEncoding:NSStringEncodingConversionAllowLossy];
                                            UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONPOSTTOWALL, statusMessageC);
                                            [self destroyOverlayViewController];
                                        }];
        }];
    }*/
}

-(void) initializeRequestConnection {
    if(self.requestConnection != nil) {
        [self.requestConnection release];
        self.requestConnection = nil;
    }
    self.requestConnection = [[FBRequestConnection alloc] initWithTimeout:timeBeforeCancelling];
}

-(void) startShareRequestWithPhoto:(UIImage *) image parameters:(NSMutableDictionary *) params completionBlock:(void (^)(BOOL)) callback {
    NSLog(@"%s", __func__);
    /*NSString *statusMessage = @"Request Started";
     const char *statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
     UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONREQUEST_STARTED, statusMessageC);*/
    [self initializeRequestConnection];
    [self requestForUploadPhoto: image completionBlock:^(NSString *photoID){
        if(photoID != nil) {
            [self initializeRequestConnection];
            [self requestPhotoInformationWithID:photoID completionBlock:^(NSString *imageLink){
                if(imageLink != nil) {
                    [self initializeRequestConnection];
                    if([params objectForKey:@"picture"] != nil) [params removeObjectForKey:@"picture"];
                    [params setObject:imageLink forKey:@"picture"];
                    [self requestWithGraphPath:@"me/feed" parameters:params HTTPMethod:@"POST" completionBlock:^(BOOL isSuccess){
                        callback(isSuccess);
                        [self.requestConnection release];
                        self.requestConnection = nil;
                    }];
                } else {
                    callback(NO);
                    [self.requestConnection release];
                    self.requestConnection = nil;
                }
            }];
        } else {
            callback(NO);
            [self.requestConnection release];
            self.requestConnection = nil;
        }
    }];
}

-(void) requestForUploadPhoto:(UIImage *) image completionBlock:(void (^)(NSString*)) callback {
    NSLog(@"%s", __func__);
    if(self.requestConnection != nil) {
        [self.requestConnection addRequest:[FBRequest requestForUploadPhoto:image] completionHandler:^(FBRequestConnection *connection, id result, NSError *error) {
            NSLog(@"Result: %@ Error: %@", result, [error localizedDescription]);
            if(!error){
                callback([result objectForKey:@"id"]);
                NSError * jsonReadError;
                NSData * jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&jsonReadError];
                NSString * jsonString = [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
                NSLog(@"%@", jsonString);
            } else {
                callback(nil);
                /*NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", result, [error localizedDescription]];
                 const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
                 UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);*/
            }
        }];
        [self.requestConnection start];
    }
}

-(void) requestPhotoInformationWithID:(NSString *) path completionBlock:(void (^)(NSString*)) callback {
    NSLog(@"%s", __func__);
    if(self.requestConnection !=nil) {
        [self.requestConnection addRequest:[FBRequest requestForGraphPath:path] completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
            NSLog(@"Result: %@ Error: %@", result, [error localizedDescription]);
            if(!error) {
                //Get the pictureLink
                callback([result objectForKey:@"picture"]);
                NSError * jsonReadError;
                NSData * jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&jsonReadError];
                NSString * jsonString = [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
                NSLog(@"%@", jsonString);
            } else {
                callback(nil);
                /*NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", result, [error localizedDescription]];
                 const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
                 UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);*/
            }
        }];
        [self.requestConnection start];
    }
}

-(void) requestWithGraphPath:(NSString*)path parameters:(NSDictionary*)params HTTPMethod:(NSString*)method completionBlock:(void (^)(BOOL))callback {
    NSLog(@"%s", __func__);
    if(self.requestConnection != nil) {
        [self.requestConnection addRequest:[FBRequest requestWithGraphPath:path parameters:params HTTPMethod:method] completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
            NSLog(@"[FacebookManager requestGraphPath] Result: %@ Error: %@", result, [error localizedDescription]);
            if(!error){
                callback(YES);
                /*NSError * jsonReadError;
                 NSData * jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&jsonReadError];
                 NSString * jsonString = [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
                 NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", jsonString, [error localizedDescription]];
                 const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
                 UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);*/
            } else {
                callback(NO);
                /*NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", result, [error localizedDescription]];
                 const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
                 UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);*/
            }
        }];
        [self.requestConnection start];
    }
}

-(void)requestGraphPath:(NSString*)path {
    NSLog(@"%s", __func__);
    [self performPublishAction:^{
        [FBRequestConnection startWithGraphPath:path
                              completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
                                  NSLog(@"[FacebookManager requestGraphPath] Result: %@ Error: %@", result, [error localizedDescription]);
                                  if(result != nil) {
                                      NSError * jsonReadError;
                                      NSData * jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&jsonReadError];
                                      NSString * jsonString = [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
                                      NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", jsonString, [error localizedDescription]];
                                      const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
                                      UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);
                                  }
                              }];
    }];
}

-(void)requestGraphPath:(NSString*)path parameters:(NSDictionary*)params HTTPMethod:(NSString*)method completionBlock:(void (^)(BOOL))callback {
    NSLog(@"%s", __func__);
    [FBRequestConnection startWithGraphPath:path
                                 parameters:params
                                 HTTPMethod:method
                          completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
                              NSLog(@"[FacebookManager requestGraphPath] Result: %@ Error: %@", result, [error localizedDescription]);
                              callback(error==nil);
                          }];
}

-(void) presentWebDialogRequestModallyWithMessage:(NSString *) message title:(NSString *) title parameters:(NSDictionary *) parameters
                                         callback:(void(^)(BOOL)) callback {
    NSLog(@"%s", __func__);
    /*FBFrictionlessRecipientCache *friendCache = [[[FBFrictionlessRecipientCache alloc] init] autorelease];
     [friendCache prefetchAndCacheForSession:self.session];*/
    [FBWebDialogs presentRequestsDialogModallyWithSession:self.session message:message title:title parameters:parameters
                                                  handler:^(FBWebDialogResult result, NSURL *resultURL, NSError *error){
                                                      NSLog(@"%@", [resultURL absoluteString]);
                                                      if(error == NULL) {
                                                          switch(result) {
                                                              case FBWebDialogResultDialogCompleted:
                                                                  NSLog(@"Request Sent");
                                                                  callback(![[resultURL absoluteString] isEqualToString:@"fbconnect://success"]);
                                                                  break;
                                                              case FBWebDialogResultDialogNotCompleted:
                                                                  NSLog(@"Request Cancelled");
                                                                  callback(false);
                                                                  break;
                                                              default:
                                                                  break;
                                                          }
                                                      } else {
                                                          callback(false);
                                                      }
                                                  }];
    
    /*UIImageView *imageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"fbwebDialogs_closeButton"]];
     UIView *tempFrontView = [[[[UIApplication sharedApplication] keyWindow] subviews] lastObject];
     [imageView setCenter:CGPointMake( tempFrontView.bounds.size.width * 0.94f, tempFrontView.bounds.size.height * 0.04f)];
     [tempFrontView addSubview:imageView];
     [imageView release];*/
}

#pragma mark - Uploading photo and getting its info.

-(void) uploadPhotoWithImage:(UIImage *) image completionBlock:(void (^)(NSString*)) callback {
    NSLog(@"%s", __func__);
    if (!image) return;
    NSString *statusMessage = @"Request Started";
    const char *statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
    UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONREQUEST_STARTED, statusMessageC);
    [FBRequestConnection startForUploadPhoto:image
                           completionHandler:^(FBRequestConnection *connection, id result, NSError *error) {
                               NSLog(@"[FacebookManager uploadPhotoWithImage] Result: %@ Error: %@", result, [error localizedDescription]);
                               if(!error){
                                   [self getPhotoInfoFromGraphPathWithID:[result objectForKey:@"id"] completionBlock:callback];
                               } else {
                                   callback(nil);
                                   NSError * jsonReadError;
                                   NSData * jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&jsonReadError];
                                   NSString * jsonString = [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
                                   NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", jsonString, [error localizedDescription]];
                                   const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
                                   UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);
                               }
                           }];
}

-(void) uploadPhotoWithImage:(UIImage *) image {
    [self uploadPhotoWithImage:image completionBlock:nil];
}

-(void) getPhotoInfoFromGraphPathWithID:(NSString *) path completionBlock:(void (^)(NSString*)) callback {
    NSLog(@"%s", __func__);
    [FBRequestConnection startWithGraphPath:path completionHandler:^(FBRequestConnection *connection, id result, NSError *error){
        NSLog(@"[FacebookManager uploadPhoto] Result: %@ Error: %@", result, [error localizedDescription]);
        if(!error) {
            //Get the pictureLink
            callback([result objectForKey:@"picture"]);
        } else {
            NSError * jsonReadError;
            NSData * jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&jsonReadError];
            NSString * jsonString = [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
            NSString * statusMessage = [NSString stringWithFormat:@"%@|%@", jsonString, [error localizedDescription]];
            const char * statusMessageC = [statusMessage cStringUsingEncoding:NSUTF8StringEncoding];
            UnitySendMessage(UNITY_FBLISTENER_GAMEOBJECT_NAME, UNITY_FBLISTENER_ONGRAPHRESPONSE, statusMessageC);
        }
    }];
}

#pragma mark Private Methods

// Convenience method to perform some action that requires the "publish_actions" permissions.
- (void) performPublishAction:(void (^)(void)) action {
    // we defer request for permission to post to the moment of post, then we check for the permission])
    if ([FBSession.activeSession.permissions indexOfObject:@"publish_actions"] == NSNotFound) {
        [FBSession.activeSession requestNewPublishPermissions:[NSArray arrayWithObject:@"publish_actions"]
                                              defaultAudience:FBSessionDefaultAudienceFriends
                                            completionHandler:^(FBSession *session, NSError *error){
                                                if(!error) action();
                                            }];
    } else {
        action();
    }
}

-(void)createOverlayViewController {
    CGSize windowSize = [UIScreen mainScreen].bounds.size;
    self.overlayViewController = [[UIViewController alloc] initWithNibName:nil bundle:nil];
    [[[UIApplication sharedApplication] keyWindow] addSubview:self.overlayViewController.view];
    self.overlayViewController.view.frame = CGRectMake(0, 0, windowSize.width, windowSize.height);
}

-(void)destroyOverlayViewController {
    [self.overlayViewController.view removeFromSuperview];
    [self.overlayViewController release];
    self.overlayViewController = nil;
}

@end
