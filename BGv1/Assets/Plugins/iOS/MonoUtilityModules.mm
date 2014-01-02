//
//  MonoUtilityModules.cpp
//  Unity-iPhone
//
//  Created by Ranier Montalbo on 5/22/13.
//
//

#include "MonoUtilityModules.h"

extern "C"
{
    MonoDomain *mono_domain_get();
    MonoAssembly *mono_domain_assembly_open(MonoDomain *domain, const char *assemblyName);
    MonoImage *mono_assembly_get_image(MonoAssembly *assembly);
    MonoMethodDesc *mono_method_desc_new(const char *methodString, gboolean useNamespace);
    MonoMethodDesc *mono_method_desc_free(MonoMethodDesc *desc);
    MonoMethod *mono_method_desc_search_in_image(MonoMethodDesc *methodDesc, MonoImage *image);
    MonoObject *mono_runtime_invoke(MonoMethod *method, void *obj, void **params, MonoObject **exc);
    MonoString *mono_string_new(MonoDomain *domain, const char *string);
}

@implementation MonoUtility
+(MonoUtility*)getInstance {
    static MonoUtility* instance;
    if(instance == nil) {
        instance = [[MonoUtility alloc] init];
    }
    return instance;
}

-(MonoUtility*)init {
    if(self = [super init]){
        domain = mono_domain_get();
        NSString* assemblyPath = [[[NSBundle mainBundle] bundlePath] stringByAppendingString:@"/Data/Managed/Assembly-CSharp.dll"];
        NSLog(@"Loading CSharp module at %@", assemblyPath);
        assembly = mono_domain_assembly_open(domain, assemblyPath.UTF8String);
        image = mono_assembly_get_image(assembly);
    }
    return self;
}

-(void)dealloc {
    delete domain;
    delete assembly;
    delete image;
    [super dealloc];
}

-(MonoMethod*)getMethod:(NSString*)methodName {
    MonoMethodDesc *desc = mono_method_desc_new(methodName.UTF8String, FALSE);
    MonoMethod *method = mono_method_desc_search_in_image(desc, image);
    mono_method_desc_free(desc);
    return method;
}

-(MonoObject*)invokeMethod:(MonoMethod *)method withArgs:(void **)args {
    return mono_runtime_invoke(method, NULL, args, NULL);
}

-(MonoString*)createMonoString:(const char*)string {
    return mono_string_new(domain, string);
}

#pragma mark Utility Methods

+(char*) copyString: (const char*) string {
    if (string == NULL) return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

+(NSString*) stringFromCString:(const char *)string {
    return [NSString stringWithUTF8String: string ? string : ""];
}

@end