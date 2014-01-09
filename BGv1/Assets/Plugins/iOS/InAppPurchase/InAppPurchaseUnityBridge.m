//
//  InAppPurchaseUnityBridge.m
//  Unity-iPhone
//
//  Created by Raneiro Montalbo on 3/12/13.
//
//

#import "PurchasingManager.h"
#import "MonoUtilityModules.h"

#define PURCHASING_UNLOCK_METHOD_NAME @"InAppPurchaseManager:Unlock(string,bool,string)"
#define MESSAGE_SEPARATOR @"^"

void _purchaseProductWithId(const char* productId) {
    NSString *itemId = [NSString stringWithUTF8String:productId];
    [[PurchasingManager sharedInstance] purchaseItemId:itemId withResultHandler:^(NSString* productIds, BOOL result, NSString* errors) {
        MonoObject* methodName = [[MonoUtility getInstance] createMonoString:"_purchaseProductWithId"];
        MonoObject* message = [[MonoUtility getInstance] createMonoString:[[NSString stringWithFormat:@"%@%@%@", productIds, MESSAGE_SEPARATOR, errors] UTF8String]];
        BOOL success = result;
        void* args[3] = {methodName, &success, message};
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:PURCHASING_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

void _restorePurchasedItems(){
    [[PurchasingManager sharedInstance] restorePurchasedItems: ^(NSString* productIds, BOOL result, NSString* errors) {
        MonoObject* methodName = [[MonoUtility getInstance] createMonoString:"_restorePurchasedItems"];
        MonoObject* message = [[MonoUtility getInstance] createMonoString:[[NSString stringWithFormat:@"%@%@%@", productIds, MESSAGE_SEPARATOR, errors] UTF8String]];
        BOOL success = result;
        void* args[3] = {methodName, &success, message};
        MonoMethod* callback = [[MonoUtility getInstance] getMethod:PURCHASING_UNLOCK_METHOD_NAME];
        [[MonoUtility getInstance] invokeMethod:callback withArgs:args];
    }];
}

