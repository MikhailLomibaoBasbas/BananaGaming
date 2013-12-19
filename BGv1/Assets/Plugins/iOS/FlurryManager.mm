//
//  FlurryManager.m
//  Barve Eggs
//
//  Created by Osipov Stanislav on 1/23/13.
//
//

#import "FlurryManager.h"
#import "Flurry.h"
#import "Unity3d.h"

@implementation FlurryManager

static FlurryManager * _instance;

+ (FlurryManager *) instance {
    
    if (_instance == nil){
        _instance = [[FlurryManager alloc] init];
    }
    
    return _instance;
}

-(id) init {
    if(self = [super init]){
        _lastAction = @"none";
        _lastParams = @"none";
    }
    return self;
}


-(void) setLastAction:(NSString *)actionId params:(NSString *)prs {
    _lastAction = actionId;
    _lastParams = prs;
}

-(void) sessionFinish {
    
    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: _lastAction, @"last_action", _lastParams, @"params",  nil];
    
    [Flurry logEvent:@"eggs_session_finish" withParameters:dict];
    
    NSLog(@"Flurry: session finished event sended");
    NSLog(@"Flurry: last Action: %@", _lastAction);
}



@end

extern "C" {
    void _connect (char* appId)  {
        [Flurry startSession:[Unity3d charToNSString:appId]];
        NSLog(@"FLURRY CONNECT API KEY: %@", [Unity3d charToNSString:appId]);
    }
    
    void _logEvent(char* eventName, BOOL isTimed) {
        NSString *event = [Unity3d charToNSString:eventName];
        [Flurry logEvent:event timed:isTimed];
        NSLog(@"_logEvent");
    }
    
    
    void _logEventWihParams(char* eventName, char* keys, char* values, BOOL isTimed) {
        NSString* str = [Unity3d charToNSString:keys];
        NSArray *keyItems = [str componentsSeparatedByString:@","];
        
        str = [Unity3d charToNSString:values];
        NSArray *valItems = [str componentsSeparatedByString:@","];
        
        NSString *event = [Unity3d charToNSString:eventName];
        
        NSDictionary *dic = [[NSDictionary alloc] initWithObjects:valItems forKeys:keyItems];
        
        [Flurry logEvent:event withParameters:dic timed:isTimed];
        
        
        NSLog(@"count %i", dic.count);
        NSLog(@"_logEventWihParams");
    }
    
    void _setUserId(char* UID) {
        [Flurry setUserID:[Unity3d charToNSString:UID]];
    }
    
    void _setAge(int age) {
        [Flurry setAge:age];
    }
    
    
    
    void _endTimedEvent(char* eventName, char* keys, char* values) {
        
        NSString* str = [Unity3d charToNSString:keys];
        NSArray *keyItems = [str componentsSeparatedByString:@","];
        
        str = [Unity3d charToNSString:values];
        NSArray *valItems = [str componentsSeparatedByString:@","];
        
        NSString *event = [Unity3d charToNSString:eventName];
        
        NSDictionary *dic = [[NSDictionary alloc] initWithObjects:valItems forKeys:keyItems];
        
        NSLog(@"count %i", dic.count);
        
        
        
        
        
        [Flurry endTimedEvent:event withParameters:dic];
        NSLog(@"_endTimedEvent");
    }
    
    void _setGender(char* gender) {
        // Valid inputs are m (male) or f (female)
        [Flurry setGender:[Unity3d charToNSString:gender]];
    }
    
    void _sessionEnd() {
        
    }
}

