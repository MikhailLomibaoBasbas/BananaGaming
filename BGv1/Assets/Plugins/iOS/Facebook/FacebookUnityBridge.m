//
//  FacebookUnityBridge.m
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 1/8/13.
//
//

#import <UIKit/UIKit.h>
#import "FacebookUnityManager.h"
#import "MonoUtilityModules.h"

#define FACEBOOK_UNLOCK_METHOD_NAME @"Facebook:Unlock(string,bool,string)"

void _fbOpenSession(const char* fbAppId, struct ObjCResult *status) {
    [[FacebookUnityManager sharedManager] facebookReconnect:[NSString stringWithUTF8String:fbAppId] completionBlock:^(BOOL result){
        status->success = result;
        status->message = [MonoUtility copyString:nil];
        
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_fbOpenSession"],
            &result,
            [[MonoUtility getInstance] createMonoString:""],
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:FACEBOOK_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

void _fbCloseSession() {
    [[FacebookUnityManager sharedManager] closeSession];
}

bool _fbHasOpenSession() {
    return [[FacebookUnityManager sharedManager] hasOpenSession];
}

void _fbGetUserInfo(struct ObjCResult *status) {
    [[FacebookUnityManager sharedManager] getUserInfoWithCompletionBlock:^(BOOL result){
        status->success = result;
        status->message = [MonoUtility copyString:nil];
        
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_fbGetUserInfo"],
            &result,
            [[MonoUtility getInstance] createMonoString:""],
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:FACEBOOK_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

const char * _fbGetUserId() {
    return [MonoUtility copyString:[[[FacebookUnityManager sharedManager] getUserId] UTF8String]];
}

const char * _fbGetUserName() {
    return [MonoUtility copyString:[[[FacebookUnityManager sharedManager] getUserName] UTF8String]];
}

void _fbGetUserFriends(struct ObjCResult *status) {
    [[FacebookUnityManager sharedManager] getUserFriendsWithCompletionBlock:^(BOOL result){
        status->success = result;
        status->message = [MonoUtility copyString:nil];
        
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_fbGetUserFriends"],
            &result,
            [[MonoUtility getInstance] createMonoString:""],
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:FACEBOOK_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

int _fbGetUserFriendsCount() {
    return [[FacebookUnityManager sharedManager] getUserFriendsCount];
}

const char * _fbGetUserFriendIdAtIndex(int index) {
    return [MonoUtility copyString:[[[FacebookUnityManager sharedManager] getUserFriendIdAtIndex:index] UTF8String]];
}

const char * _fbGetUserFriendNameAtIndex(int index) {
    return [MonoUtility copyString:[[[FacebookUnityManager sharedManager] getUserFriendNameAtIndex:index] UTF8String]];
}

void _fbPostUserFeed(const char* message,
                     const char* name,
                     const char* caption,
                     const char* description,
                     const char* link,
                     const char* picture,
                     struct ObjCResult* status) {
    NSDictionary* postParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                [NSString stringWithUTF8String:message],    @"message",
                                [NSString stringWithUTF8String:name],       @"name",
                                [NSString stringWithUTF8String:caption],    @"caption",
                                [NSString stringWithUTF8String:description],@"description",
                                [NSString stringWithUTF8String:link],       @"link",
                                [NSString stringWithUTF8String:picture],    @"picture",
                                nil];
    [[FacebookUnityManager sharedManager] postUserFeed:postParams completionBlock:^(BOOL result) {
        status->success = result;
        status->message = [MonoUtility copyString:nil];
        
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_fbPostUserFeed"],
            &result,
            [[MonoUtility getInstance] createMonoString:""],
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:FACEBOOK_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

void _fbPostUserFeedWithActions(const char* message,
                                const char* name,
                                const char* caption,
                                const char* description,
                                const char* link,
                                const char* picture,
                                const char* actions,
                                struct ObjCResult* status) {
    NSString * actionString = [NSString stringWithUTF8String:actions];
    NSMutableArray * actionLinks = [NSMutableArray array];
    if(actions != NULL) {
        NSArray * actionNameLinkPairs = [actionString componentsSeparatedByString:@","];
        for (NSString * nameLinkPair in actionNameLinkPairs) {
            NSArray * nameLink = [nameLinkPair componentsSeparatedByString:@"|"];
            [actionLinks addObject:[NSDictionary dictionaryWithObjectsAndKeys:nameLink[0], @"name", nameLink[1], @"link", nil]];
        }
    }
    
    NSDictionary* postParams = [NSDictionary dictionaryWithObjectsAndKeys:
                                [NSString stringWithUTF8String:message],    @"message",
                                [NSString stringWithUTF8String:name],       @"name",
                                [NSString stringWithUTF8String:caption],    @"caption",
                                [NSString stringWithUTF8String:description],@"description",
                                [NSString stringWithUTF8String:link],       @"link",
                                [NSString stringWithUTF8String:picture],    @"picture",
                                actionLinks,                                @"actions",
                                nil];
    
    [[FacebookUnityManager sharedManager] postUserFeed:postParams completionBlock:^(BOOL result) {
        status->success = result;
        status->message = [MonoUtility copyString:nil];
        
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_fbPostUserFeedWithActions"],
            &result,
            [[MonoUtility getInstance] createMonoString:""],
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:FACEBOOK_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

void _fbPostStatusUpdate() {
    NSString *fbMessage = [[NSUserDefaults standardUserDefaults] stringForKey:@"fb_status_message"];
    [[FacebookUnityManager sharedManager] postStatusUpdate:fbMessage];
}

void _fbRequestGraphPath() {
    NSUserDefaults * defaults = [NSUserDefaults standardUserDefaults];
    NSString * path = [defaults stringForKey:@"fb_request_path"];
    NSString * method = [defaults stringForKey:@"fb_request_method"];
    NSString * params = [defaults stringForKey:@"fb_request_params"];
    
    if(method == NULL || params == NULL) {
        [[FacebookUnityManager sharedManager] requestGraphPath:path];
    }else{
        NSError * jsonReadError;
        NSDictionary * jsonData = [NSJSONSerialization JSONObjectWithData:[params dataUsingEncoding:NSUTF8StringEncoding]
                                                                  options:NSJSONReadingMutableContainers
                                                                    error:&jsonReadError];
        if(jsonReadError != NULL)
            NSLog(@"[%s] %@", __func__, [jsonReadError localizedDescription]);
        else
            [[FacebookUnityManager sharedManager] requestGraphPath:path parameters:jsonData HTTPMethod:method completionBlock:nil];
    }
}

void _fbPresentWebDialogRequest (const char* message, const char* title, struct ObjCResult *status) {
    NSDictionary *dictionary = [NSDictionary dictionary];
    [[FacebookUnityManager sharedManager] presentWebDialogRequestModallyWithMessage:[NSString stringWithUTF8String:message]
                title:[NSString stringWithUTF8String:title] parameters:dictionary
                callback:^(BOOL result){
                    status->success = result;
                    status->message = [MonoUtility copyString:nil];
                                                                    
                    void* args[3] = {
                        [[MonoUtility getInstance] createMonoString:"_fbPresentWebDialogRequest"],
                        &result,
                        [[MonoUtility getInstance] createMonoString:""],
                    };
                   
                    MonoMethod* callback = [[MonoUtility getInstance] getMethod:FACEBOOK_UNLOCK_METHOD_NAME];
                   [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
          }];
}