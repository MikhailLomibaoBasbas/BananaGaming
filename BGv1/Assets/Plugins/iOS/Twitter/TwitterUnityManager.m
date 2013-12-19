//
//  TwitterUnityManager.m
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 1/22/13.
//
//

#import "TwitterUnityManager.h"

@implementation TwitterUnityManager

+(TwitterUnityManager*)sharedManager {
    static TwitterUnityManager * sharedInstance;
    if(sharedInstance == NULL)
        sharedInstance = [[TwitterUnityManager alloc] init];
    return sharedInstance;
}

-(void) tweetComposeWithViewController:(UIViewController *) viewController text:(NSString *) text image:(UIImage *) image url:(NSURL *) url completion:(void (^)(TwitterStatus))completion {
    NSLog(@"%s", __func__);
    if([TWTweetComposeViewController canSendTweet]) {
        TWTweetComposeViewController *tweet = [[[TWTweetComposeViewController alloc] init] autorelease];
        [tweet setInitialText:text];
        [tweet addImage:image];
        [tweet addURL:url];
        [tweet setCompletionHandler:^(TWTweetComposeViewControllerResult result){
            switch (result) {
                case SLComposeViewControllerResultDone:
                        completion(TweetSuccess);
                    break;
                case SLComposeViewControllerResultCancelled:
                        completion(TweetCancelled);
                    break;
                default:
                    break;
            }
            [tweet dismissViewControllerAnimated:YES completion:^{
                [viewController.view removeFromSuperview];
            }];
            
        }];
        [viewController presentViewController:tweet animated:YES completion:nil];
    } else {
         completion(TweetNoAccount);
    }
}

-(void)getRequest:(NSString *)url params:(NSDictionary *)params completionBlock:(TwitterResponseBlock)callback {
    NSLog(@"%s", __func__);
    [self createRequest:url params:params method:TWRequestMethodGET completionBlock:callback];
}

-(void)postRequest:(NSString *)url params:(NSDictionary *)params completionBlock:(TwitterResponseBlock)callback {
    NSLog(@"%s", __func__);
    [self createRequest:url params:params method:TWRequestMethodPOST completionBlock:callback];
}

-(void)deleteRequest:(NSString *)url params:(NSDictionary *)params completionBlock:(TwitterResponseBlock)callback {
    NSLog(@"%s", __func__);
    [self createRequest:url params:params method:TWRequestMethodDELETE completionBlock:callback];
}

-(void)getAccountInfoWithCompletionBlock:(TwitterResponseBlock)callback {
    //  First, we need to obtain the account instance for the user's Twitter account
    ACAccountStore *store = [[ACAccountStore alloc] init];
    ACAccountType *twitterAccountType =
    [store accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
    
    //  Request permission from the user to access the available Twitter accounts
    [store requestAccessToAccountsWithType:twitterAccountType
                     withCompletionHandler:
     ^(BOOL granted, NSError *error) {
         NSMutableDictionary *params = [NSMutableDictionary dictionary];
         [params setObject:callback forKey:@"callback"];
         if(!granted){
             [params setObject:[NSNumber numberWithBool:granted] forKey:@"result"];
             [params setObject:[error localizedDescription] forKey:@"message"];
             [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
             //callback(granted, [error localizedDescription]);
         }else{
             // Grab the available accounts
             NSArray *twitterAccounts = [store accountsWithAccountType:twitterAccountType];
             if(twitterAccounts.count > 0) {
                 [TwitterUnityManager sharedManager].account = [twitterAccounts objectAtIndex:0];
                 [params setObject:[NSNumber numberWithBool:YES] forKey:@"result"];
                 [params setObject:@"Twitter account successfully loaded" forKey:@"message"];
                 [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
                 //callback(YES, @"Twitter account successfully loaded");
             }else{
                 [params setObject:[NSNumber numberWithBool:NO] forKey:@"result"];
                 [params setObject:@"No twitter account available" forKey:@"message"];
                 [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
                 //callback(NO, @"No twitter account available");
             }
         }
     }];
}

-(void)invokeCallback:(NSDictionary*)params {
    TwitterResponseBlock callback = [params objectForKey:@"callback"];
    BOOL result = [[params objectForKey:@"result"] boolValue];
    NSString *message = [params objectForKey:@"message"];
    callback(result, message);
}

-(NSString*)getUserId {
    return self.account.identifier;
}

-(NSString*)getUserName {
    return self.account.username;
}

-(void)createRequest:(NSString*)path params:(NSDictionary*)params method:(TWRequestMethod)method completionBlock:(TwitterResponseBlock)callback {
    if(self.account != nil) {
        [self getAccountInfoWithCompletionBlock:^(BOOL result, NSString* message) {
            if(result) {
                [self sendRequest:path params:params method:method completionBlock:callback];
            }else{
                NSMutableDictionary* params = [NSMutableDictionary dictionary];
                [params setObject:callback forKey:@"callback"];
                [params setObject:[NSNumber numberWithBool:result] forKey:@"result"];
                [params setObject:message forKey:@"message"];
                [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
                //callback(result, message);
            }
        }];
    }else{
        [self sendRequest:path params:params method:method completionBlock:callback];
    }
}

-(void)sendRequest:(NSString*)path params:(NSDictionary*)params method:(TWRequestMethod)method completionBlock:(TwitterResponseBlock)callback {
    // Build the request with our parameter
    TWRequest *request = [[TWRequest alloc] initWithURL:[NSURL URLWithString:path]
                                             parameters:params
                                          requestMethod:method];
    
    // Attach the account object to this request
    [request setAccount:self.account];
    
    [request performRequestWithHandler:
     ^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error) {
         if (!responseData) {
             // inspect the contents of error
             NSLog(@"[TwitterUnityManager:createRequest] %@", [error localizedDescription]);
             NSMutableDictionary* params = [NSMutableDictionary dictionary];
             [params setObject:callback forKey:@"callback"];
             [params setObject:[NSNumber numberWithBool:NO] forKey:@"result"];
             [params setObject:[error localizedDescription] forKey:@"message"];
             [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
             //callback(NO, [error localizedDescription]);
         }
         else {
             NSError *jsonError;
             NSArray *timeline = [NSJSONSerialization JSONObjectWithData:responseData options:NSJSONReadingMutableLeaves error:&jsonError];
             if (timeline) {
                 // at this point, we have an object that we can parse
                 NSLog(@"%@", timeline);
                 NSString * resultMessage = [[[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding] autorelease];
                 NSMutableDictionary* params = [NSMutableDictionary dictionary];
                 [params setObject:callback forKey:@"callback"];
                 [params setObject:[NSNumber numberWithBool:YES] forKey:@"result"];
                 [params setObject:resultMessage forKey:@"message"];
                 [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
                 //callback(YES, resultMessage);
             }
             else {
                 // inspect the contents of jsonError
                 NSMutableDictionary* params = [NSMutableDictionary dictionary];
                 [params setObject:callback forKey:@"callback"];
                 [params setObject:[NSNumber numberWithBool:NO] forKey:@"result"];
                 [params setObject:[jsonError localizedDescription] forKey:@"message"];
                 [self performSelectorOnMainThread:@selector(invokeCallback:) withObject:params waitUntilDone:NO];
                 //callback(NO, [jsonError localizedDescription]);
             }
         }
     }];
}

@end
