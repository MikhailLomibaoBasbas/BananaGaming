//
//  FlurryManager.h
//  Barve Eggs
//
//  Created by Osipov Stanislav on 1/23/13.
//
//

#import <Foundation/Foundation.h>

@interface FlurryManager : NSObject {
    NSString * _lastAction;
    NSString * _lastParams;
}

+ (FlurryManager *) instance;

- (void)setLastAction: (NSString *) actionId params: (NSString*) prs;

- (void)sessionFinish;

@end
