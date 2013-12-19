//
//  TwitterUnityBridge.m
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 1/22/13.
//
//

#import "TwitterUnityManager.h"
#import "MonoUtilityModules.h"

#define TWITTER_UNLOCK_METHOD_NAME @"Twitter:Unlock(string,bool,string)"

void _twitterGetAccountInfo(struct ObjCResult* status) {
    [[TwitterUnityManager sharedManager] getAccountInfoWithCompletionBlock:^(BOOL result, NSString* message) {
        status->success = result;
        status->message = [MonoUtility copyString:[message UTF8String]];
        
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_twitterGetAccountInfo"],
            &result,
            [[MonoUtility getInstance] createMonoString:[message UTF8String]]
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

const char* _twitterGetAccountId() {
    return [MonoUtility copyString:[[[TwitterUnityManager sharedManager] getUserId] UTF8String]];
}

const char* _twitterGetAccountName() {
    return [MonoUtility copyString:[[[TwitterUnityManager sharedManager] getUserName] UTF8String]];
}

void _twitterPostRequest(const char*url, const char* parameters, struct ObjCResult* status) {
    NSError * jsonReadError = nil;
    NSString * urlString = [NSString stringWithUTF8String:url];
    NSString * paramString = [NSString stringWithUTF8String:parameters];
    NSDictionary * params = [NSJSONSerialization JSONObjectWithData:[paramString dataUsingEncoding:NSUTF8StringEncoding]
                                                            options:NSJSONReadingMutableContainers
                                                              error:&jsonReadError];
    NSLog(@"_twitterPostRequest: %@", paramString);
    
    if(jsonReadError != NULL) {
        NSLog(@"[%s] %@", __func__, jsonReadError);
        status->success = false;
        status->message = [MonoUtility copyString:[[jsonReadError localizedDescription] UTF8String]];
        
        BOOL failed = false;
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_twitterPostRequest"],
            &failed,
            [[MonoUtility getInstance] createMonoString:[[jsonReadError localizedDescription] UTF8String]]
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }else{
        [[TwitterUnityManager sharedManager] postRequest:urlString params:params completionBlock:^(BOOL result, NSString* message) {
            status->success = result;
            status->message = [MonoUtility copyString:[message UTF8String]];
            
            void* args[3] = {
                [[MonoUtility getInstance] createMonoString:"_twitterPostRequest"],
                &result,
                [[MonoUtility getInstance] createMonoString:[message UTF8String]]
            };
            MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
            [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
        }];
    }
}

void _twitterGetRequest(const char* url, const char* parameters, struct ObjCResult* status) {
    NSError * jsonReadError = nil;
    NSString * urlString = [NSString stringWithUTF8String:url];
    NSString * paramString = [NSString stringWithUTF8String:parameters];
    NSDictionary * params = [NSJSONSerialization JSONObjectWithData:[paramString dataUsingEncoding:NSUTF8StringEncoding]
                                                            options:NSJSONReadingMutableContainers
                                                              error:&jsonReadError];
    NSLog(@"_twitterGetRequest: %@", paramString);
    if(jsonReadError != nil) {
        NSLog(@"[%s] %@", __func__, jsonReadError);
        status->success = false;
        status->message = [MonoUtility copyString:[[jsonReadError localizedDescription] UTF8String]];
        
        BOOL failed = false;
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_twitterGetRequest"],
            &failed,
            [[MonoUtility getInstance] createMonoString:[[jsonReadError localizedDescription] UTF8String]]
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }else{
        [[TwitterUnityManager sharedManager] getRequest:urlString params:params completionBlock:^(BOOL result, NSString* message) {
            status->success = result;
            status->message = [MonoUtility copyString:[message UTF8String]];
            
            void* args[3] = {
                [[MonoUtility getInstance] createMonoString:"_twitterGetRequest"],
                &result,
                [[MonoUtility getInstance] createMonoString:[message UTF8String]]
            };
            MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
            [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
        }];
    }
}

void _twitterDeleteRequest(const char* url, const char* parameters, struct ObjCResult* status) {
    NSError * jsonReadError = nil;
    NSString * urlString = [NSString stringWithUTF8String:url];
    NSString * paramString = [NSString stringWithUTF8String:parameters];
    NSDictionary * params = [NSJSONSerialization JSONObjectWithData:[paramString dataUsingEncoding:NSUTF8StringEncoding]
                                                            options:NSJSONReadingMutableContainers
                                                              error:&jsonReadError];
    NSLog(@"_twitterDeleteRequest: %@", paramString);
    if(jsonReadError != NULL) {
        NSLog(@"[%s] %@", __func__, jsonReadError);
        status->success = false;
        status->message = [MonoUtility copyString:[[jsonReadError localizedDescription] UTF8String]];
        
        BOOL failed = false;
        void* args[3] = {
            [[MonoUtility getInstance] createMonoString:"_twitterDeleteRequest"],
            &failed,
            [[MonoUtility getInstance] createMonoString:[[jsonReadError localizedDescription] UTF8String]]
        };
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }else{
        [[TwitterUnityManager sharedManager] deleteRequest:urlString params:params completionBlock:^(BOOL result, NSString* message) {
            status->success = result;
            status->message = [MonoUtility copyString:[message UTF8String]];
            
            void* args[3] = {
                [[MonoUtility getInstance] createMonoString:"_twitterDeleteRequest"],
                &result,
                [[MonoUtility getInstance] createMonoString:[message UTF8String]]
            };
            MonoMethod* callback = [[MonoUtility getInstance] getMethod:TWITTER_UNLOCK_METHOD_NAME];
            [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
        }];
    }
}