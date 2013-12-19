//
//  PurchasingManager.h
//  Unity-iPhone
//
//  Created by Raneiro Montalbo on 3/12/13.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>


typedef void(^PurchaseResponseBlock)(NSString* productId, BOOL result, NSString* errorMessage);

@interface PurchasingManager : NSObject <SKPaymentTransactionObserver, SKProductsRequestDelegate>

@property(nonatomic, copy) PurchaseResponseBlock requestHandler;

+(PurchasingManager*)sharedInstance;

-(PurchasingManager*)init;

-(void)purchaseItemId:(NSString*)productId withResultHandler:(PurchaseResponseBlock)handler;

@end
