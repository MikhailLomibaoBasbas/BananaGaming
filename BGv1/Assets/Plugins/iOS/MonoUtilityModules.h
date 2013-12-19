//
//  MonoUtilityModules.h
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 5/22/13.
//
//
#import <UIKit/UIKit.h>

struct ObjCResult {
    bool success;
    char* message;
};

// !Mono Modules
typedef void* MonoDomain;
typedef void* MonoAssembly;
typedef void* MonoImage;
typedef void* MonoClass;
typedef void* MonoObject;
typedef void* MonoMethodDesc;
typedef void* MonoMethod;
typedef void* MonoString;
typedef int gboolean;

@interface MonoUtility : NSObject
{
    MonoDomain* domain;
    MonoAssembly* assembly;
    MonoImage* image;
}
+ (MonoUtility*) getInstance;
- (MonoUtility*) init;
- (MonoMethod*) getMethod:(NSString*) methodName;
- (MonoObject*) invokeMethod:(MonoMethod*) method withArgs:(void**) args;
- (MonoString*) createMonoString:(const char*)string;

//utility methods
+ (char*) copyString:(const char*) string;
+ (NSString*) stringFromCString:(const char*) string;
@end