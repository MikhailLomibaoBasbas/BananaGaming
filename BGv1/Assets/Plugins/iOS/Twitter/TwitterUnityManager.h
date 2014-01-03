//
//  TwitterUnityManager.h
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 1/22/13.
//
//

#import <Foundation/Foundation.h>
#import <Twitter/Twitter.h>
#import <Accounts/Accounts.h>

typedef enum {
    TweetSuccess,
    TweetCancelled,
    TweetNoAccount,
}TwitterStatus;

typedef void(^TwitterResponseBlock)(BOOL, NSString*);

@interface TwitterUnityManager : NSObject

@property(nonatomic, retain) ACAccount* account;

+(TwitterUnityManager*)sharedManager;

-(void) tweetComposeWithViewController:(UIViewController *) viewController text:(NSString *) text image:(UIImage *) image url:(NSURL *) url completion:(void (^)(TwitterStatus)) completion;

-(void)getRequest:(NSString*)url params:(NSDictionary*)params completionBlock:(TwitterResponseBlock)callback ;

-(void)postRequest:(NSString*)url params:(NSDictionary*)params completionBlock:(TwitterResponseBlock)callback ;

-(void)deleteRequest:(NSString*)url params:(NSDictionary*)params completionBlock:(TwitterResponseBlock)callback ;

-(void)getAccountInfoWithCompletionBlock:(TwitterResponseBlock)callback;

-(NSString*)getUserId;

-(NSString*)getUserName;

@end
