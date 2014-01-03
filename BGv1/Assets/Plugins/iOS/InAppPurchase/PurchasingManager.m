//
//  PurchasingManager.m
//  Unity-iPhone
//
//  Created by Raneiro Montalbo on 3/12/13.
//
//

#import "PurchasingManager.h"
#define PRODUCT_SEPARATOR @"|"

@implementation PurchasingManager

@synthesize requestHandler;

+(PurchasingManager*)sharedInstance {
    static PurchasingManager* instance;
    if(instance == nil) {
        instance = [[PurchasingManager alloc] init];
    }
    return instance;
}

-(PurchasingManager*)init {
    if(self = [super init]) {
        [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    }
    return self;
}

-(void)dealloc {
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
    if(requestHandler != NULL)
        Block_release(requestHandler);
    [super dealloc];
}

-(void)purchaseItemId:(NSString *)productId withResultHandler:(PurchaseResponseBlock)handler {
    // save handler for later
    if(requestHandler != NULL)
        Block_release(requestHandler);
    requestHandler = handler;
    
    NSSet* productIdSet = [NSSet setWithObject:productId];
    SKProductsRequest * request = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdSet];
    [request setDelegate:self];
    [request start];
}

#pragma mark PrivateMethods

-(void)showAlertWithMessage:(NSString*)message {
    UIAlertView *view = [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Purchase", @"PurchasingManager AlertView Title")
                                                   message:message
                                                  delegate:nil
                                         cancelButtonTitle:NSLocalizedString(@"OK", @"Ok Button")
                                         otherButtonTitles:nil];
    [view show];
    [view release];
}

#pragma mark SKProductsRequestDelegate

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSLog(@"%s",__FUNCTION__);
    
    if (response == nil) {
        NSLog(@"Product Response is nil");
        return;
    }
    
    if(response.invalidProductIdentifiers.count > 0) {
        NSMutableArray *productIds = [NSMutableArray array];
        NSMutableArray *errors = [NSMutableArray array];
        for (NSString *identifier in response.invalidProductIdentifiers) {
            NSString *errorMessage = [NSString stringWithFormat:@"Invalid product identifier: %@", identifier];
            NSLog(@"%@", errorMessage);
            [errors addObject:errorMessage];
            [self showAlertWithMessage:NSLocalizedString(@"StoreView_AlertMessage_InvalidProductId",nil)];
        }
        // invoke callback function and send back error message
        requestHandler([productIds componentsJoinedByString:PRODUCT_SEPARATOR], NO, [errors componentsJoinedByString:PRODUCT_SEPARATOR]);
    }else{
        for (SKProduct *product in response.products ) {
            NSLog(@"Valid product identifier: %@", product.productIdentifier);
            SKPayment *payment = [SKPayment paymentWithProduct:product];
            [[SKPaymentQueue defaultQueue] addPayment:payment];
        }
    }
}

#pragma mark SKPaymentTransactionObserver

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    NSLog(@"%s",__FUNCTION__);
    
    BOOL purchasing = YES;
    NSMutableArray *productIds = [NSMutableArray array];
    NSMutableArray *errors = [NSMutableArray array];
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
			case SKPaymentTransactionStatePurchasing: {
				NSLog(@"Payment Transaction Purchasing");
				break;
			}
			case SKPaymentTransactionStatePurchased: {
				NSLog(@"Payment Transaction END Purchased: %@", transaction.transactionIdentifier);
                [productIds addObject:transaction.payment.productIdentifier];
                [queue finishTransaction:transaction];

                [self showAlertWithMessage:NSLocalizedString(@"Transaction Succeeded",nil)];
				purchasing = NO;
				break;
			}
            case SKPaymentTransactionStateRestored: {
				NSLog(@"Payment Transaction END Restored: %@", transaction.transactionIdentifier);
                [productIds addObject:transaction.originalTransaction.payment.productIdentifier];
                [queue finishTransaction:transaction];
                
				purchasing = NO;
				break;
			}
			case SKPaymentTransactionStateFailed: {
				NSLog(@"Payment Transaction END Failed: %@ %@", transaction.transactionIdentifier, transaction.error);
                [errors addObject:transaction.error.localizedDescription];
                [queue finishTransaction:transaction];
                [self showAlertWithMessage:NSLocalizedString(@"Transaction Failed",nil)];
				purchasing = NO;
				break;
			}
        }
    }
    
    if(!purchasing) {
        requestHandler([productIds componentsJoinedByString:PRODUCT_SEPARATOR],
                       productIds.count > 0,
                       [errors componentsJoinedByString:PRODUCT_SEPARATOR]);
    }
}

-(void)paymentQueue:(SKPaymentQueue *)queue removedTransactions:(NSArray *)transactions {
    
}

@end
