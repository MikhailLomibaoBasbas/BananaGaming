//
//  GameCenterUnityBridge.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/13/13.
//
//

#import "GameCenterUnityManager.h"

bool _gcHasGameCenterViewController () {
    return [[GameCenterUnityManager sharedManager] hasGKGameCenterViewController];
}


void _gcReportScore() {
    NSUserDefaults *standardUserDefaults = [NSUserDefaults standardUserDefaults];
    NSString *bestScoreString = [standardUserDefaults objectForKey:@"RRBestScore"];
    NSString *bestDistanceString = [standardUserDefaults objectForKey:@"RRBestDistance"];
    NSString *bestTimeString = [standardUserDefaults objectForKey:@"RRBestTime"];
    NSString *bestDamageString = [standardUserDefaults objectForKey:@"RRBestDamage"];
    NSMutableDictionary *dictionary = [[NSMutableDictionary alloc] init];
    if(bestScoreString.longLongValue > 0) [dictionary setObject:bestScoreString forKey:@"RRBestScore"];
    if(bestDistanceString.longLongValue > 0) [dictionary setObject:bestDistanceString forKey:@"RRBestDistance"];
    if(bestTimeString.longLongValue > 0) [dictionary setObject:bestTimeString forKey:@"RRBestTime"];
    if(bestDamageString.longLongValue > 0) [dictionary setObject:bestDamageString forKey:@"RRBestDamage"];
    
    if(dictionary.count > 0) {
    //dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, NULL), ^{
        [[GameCenterUnityManager sharedManager] reportScore:dictionary];
    //});
    }
}

void _gcShowGameCenterViewController () {
    [[GameCenterUnityManager sharedManager] showGameCenterViewController];
}

bool _gcIsAuthenticated () {
    return [[GameCenterUnityManager sharedManager] isAuthenticated];
}

void _gcAuthenticateLocalPlayer () {
   [[GameCenterUnityManager sharedManager] authenticateLocalPlayer];
}

void _gcRetrieveTopScores () {
        [[GameCenterUnityManager sharedManager] retrieveTopScroesWithCategory:@"RRBestDistance"];
}

void _gcReportAchievements () {
    NSUserDefaults *standardDefaults = [NSUserDefaults standardUserDefaults];
    NSString *headHitID = [standardDefaults objectForKey:@"Decapitate!"];
    NSString *legsHitID = [standardDefaults objectForKey:@"Remove thou limb."];
    NSString *armsHitID = [standardDefaults objectForKey:@"Let me give you a hand."];
    NSString *totalDamageID = [standardDefaults objectForKey:@"Blood Lust"];
    NSMutableArray *achievements = [NSMutableArray array];
    if(![headHitID isEqualToString:@"0"]) [achievements addObject:headHitID];
    if(![legsHitID isEqualToString:@"0"]) [achievements addObject:legsHitID];
    if(![armsHitID isEqualToString:@"0"]) [achievements addObject:armsHitID];
    if(![totalDamageID isEqualToString:@"0"]) [achievements addObject:totalDamageID];
    if(achievements.count > 0) {
        [[GameCenterUnityManager sharedManager] reportAchievementsWithAchievements:achievements];
    }
}

void _gcResetAchievements () {
    [[GameCenterUnityManager sharedManager] resetAchievements];
}