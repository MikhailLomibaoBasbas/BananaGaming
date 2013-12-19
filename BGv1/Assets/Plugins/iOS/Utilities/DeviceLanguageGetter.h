//
//  DeviceLanguageGetter.h
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 6/28/13.
//
//

#import <Foundation/Foundation.h>

@interface DeviceLanguageGetter : NSObject

-(NSString *) getDeviceLanguage;

+(DeviceLanguageGetter *) sharedManager;

@end
