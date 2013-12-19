//
//  DeviceLanguageGetter.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 6/28/13.
//
//

#import "DeviceLanguageGetter.h"

@implementation DeviceLanguageGetter


+(DeviceLanguageGetter *) sharedManager {
    static DeviceLanguageGetter * sharedInstance;
    if(sharedInstance == NULL) {
        sharedInstance = [[DeviceLanguageGetter alloc] init];
    }
    return sharedInstance;
}

+(DeviceLanguageGetter *) init {
     if(self = [super init]){
     }
    return self;
}


-(NSString *) getDeviceLanguage {
    return [[NSLocale preferredLanguages] objectAtIndex:0];
}

@end
