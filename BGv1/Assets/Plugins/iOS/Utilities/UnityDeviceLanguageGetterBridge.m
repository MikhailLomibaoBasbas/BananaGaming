//
//  UnityDeviceLanguageGetterBridge.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 6/28/13.
//
//

#import "DeviceLanguageGetter.h"


const char * _getLanguage () {
    return [[[DeviceLanguageGetter sharedManager] getDeviceLanguage] UTF8String];
}
