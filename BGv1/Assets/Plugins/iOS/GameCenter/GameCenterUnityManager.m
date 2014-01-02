//
//  GameCenterUnityManager.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/13/13.
//
//

#import "GameCenterUnityManager.h"
#import <GameKit/GameKit.h>
#import "Reachability.h"
#import "ViewControllerWrapper.h"
#import "AppController.h"

const NSString *offPostfix = @"_Offline";

@interface GameCenterUnityManager() <GKGameCenterControllerDelegate, GKAchievementViewControllerDelegate> {
}
@property (nonatomic, readonly) dispatch_queue_t leaderBoardQueue;
@property (nonatomic, readonly) dispatch_queue_t achievementQueue;
@property (nonatomic, retain) UIViewController *viewControllerWrapper;
@property (nonatomic, retain) UIViewController *authenticationViewControllerWrapper;
@end

@implementation GameCenterUnityManager
@synthesize viewControllerWrapper;
@synthesize leaderBoardQueue, achievementQueue;

+(GameCenterUnityManager *) sharedManager {
    static GameCenterUnityManager * sharedInstance;
    if(sharedInstance == NULL) {
        static dispatch_once_t pred;
        dispatch_once(&pred, ^{
            sharedInstance = [[GameCenterUnityManager alloc] init];
        });
    }
    return sharedInstance;
    
}

-(id) init {
    self = [super init];
    if(self) {
        leaderBoardQueue = dispatch_queue_create("com.redLeaderBoard.queue", DISPATCH_QUEUE_SERIAL);
        achievementQueue = dispatch_queue_create("com.redAchievement.queue", DISPATCH_QUEUE_SERIAL);
    }
    return self;
}

-(BOOL) isConnected {
    NSLog(@"%s",__func__);
    Reachability *reachability = [Reachability reachabilityForInternetConnection];
    NetworkStatus networkStatus = [reachability currentReachabilityStatus];
    return !(networkStatus == NotReachable);
}

-(BOOL) hasGKGameCenterViewController {
    return [GKGameCenterViewController class];
}

-(BOOL) isAuthenticated {
    NSLog(@"%s", __func__);
    return [[GKLocalPlayer localPlayer] retain].authenticated;
}

-(void) cleanViewControllerWrapper {
    if(self.viewControllerWrapper != nil) {
        if([self.viewControllerWrapper isViewLoaded] && [self.viewControllerWrapper.view window]) {
            [self.viewControllerWrapper.view removeFromSuperview];
        }
        [self.viewControllerWrapper release];
        self.viewControllerWrapper = nil;
    }
}

-(void) cleanAuthenticationViewcontrollerWrapper {
    if(self.authenticationViewControllerWrapper != nil) {
        if([self.authenticationViewControllerWrapper isViewLoaded] && [self.authenticationViewControllerWrapper.view window]) {
            [self.authenticationViewControllerWrapper.view removeFromSuperview];
        }
        [self.authenticationViewControllerWrapper release];
        self.authenticationViewControllerWrapper = nil;
    }
}

-(void) presentViewController:(UIViewController *) viewControllerToPresent isAnimated:(BOOL) isAnimated {
    if(self.viewControllerWrapper != nil) {
        [self.viewControllerWrapper release];
        self.viewControllerWrapper = nil;
    }
    [AppController UnityPause:true];
    self.viewControllerWrapper = [[ViewControllerWrapper alloc] init];
    [[[UIApplication sharedApplication] keyWindow] addSubview:self.viewControllerWrapper.view];
        self.viewControllerWrapper.view.frame = CGRectMake(0, 0, [UIScreen mainScreen].bounds.size.width, [UIScreen mainScreen].bounds.size.height);
    [self.viewControllerWrapper presentViewController:viewControllerToPresent  animated:isAnimated completion:nil];
}


-(void) authenticateLocalPlayer {
    NSLog(@"%s",__func__);
   if(![GKLocalPlayer localPlayer].authenticated) {
        if(NSClassFromString(@"GKGameCenterViewController") != nil) {
            NSLog(@"IOS 6");
            [[[GKLocalPlayer localPlayer] retain] setAuthenticateHandler:
             ^(UIViewController *loginViewController, NSError *error) {
                 NSLog(@"Error: %@", [error localizedDescription]);
                 if(!error) {
                     if(loginViewController) {
                         [self cleanAuthenticationViewcontrollerWrapper];
                        [AppController UnityPause:true];
                        self.authenticationViewControllerWrapper = [[ViewControllerWrapper alloc] init];
                         [[[UIApplication sharedApplication] keyWindow] addSubview:self.authenticationViewControllerWrapper.view];
                         self.authenticationViewControllerWrapper.view.frame = CGRectMake(0, 0, [UIScreen mainScreen].bounds.size.width, [UIScreen mainScreen].bounds.size.height);
                         [self.authenticationViewControllerWrapper presentViewController:loginViewController  animated:YES completion:nil];
                     } else {
                         if(self.authenticationViewControllerWrapper != nil) {
                             [self cleanAuthenticationViewcontrollerWrapper];
                             [AppController UnityPause:false];
                         }
                     }
                 } else {
                     if(self.authenticationViewControllerWrapper != nil) {
                         //[self cleanViewController:self.authenticationViewControllerWrapper andRemoveToSuperView:YES];
                         [self cleanAuthenticationViewcontrollerWrapper];
                         [AppController UnityPause:false];
                     }
                 }
                 
            }];
        } else {
            NSLog(@"IOS 5");
            [[[GKLocalPlayer localPlayer] retain] authenticateWithCompletionHandler:^(NSError *error) {
                NSLog(@"Error: %@",[error localizedDescription]);
                if(!error) {
                    
                }
            }];
        }
    }
}

-(void) showGameCenterViewController {
    NSLog(@"%s",__func__);
    if(NSClassFromString(@"GKGameCenterViewController") != nil) {
        GKGameCenterViewController *gameCenterViewController = [[GKGameCenterViewController alloc] init];
        gameCenterViewController.gameCenterDelegate = self;
        gameCenterViewController.viewState = GKGameCenterViewControllerStateLeaderboards;
        gameCenterViewController.leaderboardTimeScope = GKLeaderboardTimeScopeToday;
        [self presentViewController:gameCenterViewController isAnimated:YES];
    }
    else {
        GKAchievementViewController *achievementViewController = [[GKAchievementViewController alloc] init];
        achievementViewController.achievementDelegate = self;
        [self presentViewController:achievementViewController isAnimated:YES];
    }
}

#pragma mark - LeaderBoard

- (void) reportScore: (NSMutableDictionary *) dictionary {
    NSLog(@"%s", __func__);
    NSLog(@"%@",dictionary);
    
    NSEnumerator *enumerator = [dictionary keyEnumerator];
    NSString *key;
    int64_t value;
    int64_t offlineValue;
    while((key = [enumerator nextObject])) {
        offlineValue = [self getOfflineHighScoreOfCategory:key];
        value = [[dictionary objectForKey:key] longLongValue];
        if(value < offlineValue) {
            value = offlineValue;
            [self deleteOfflineHighScoreOfCategory:key];
        }
        if([self isConnected]) {
            if(value > 0) {
                dispatch_async(leaderBoardQueue, ^{
                    GKScore *scoreReporter = [[GKScore alloc] initWithCategory:key];
                    NSLog(@"%lld", scoreReporter.value);
                        scoreReporter.value = value;
                        scoreReporter.context = 0;
                        [scoreReporter reportScoreWithCompletionHandler:^(NSError *error){
                            NSLog(@"%s Error: %@", __func__, [error localizedDescription]);
                            if(error) {
                                [self setOfflineHighScore:value category:key];
                            }
                        }];
                });
            }
        } else {
            [self setOfflineHighScore:value category:key];
        }
    }
}

-(void) setOfflineHighScore:(int64_t) newHigh category:(NSString *) category {
    int64_t offlineHigh = [[[NSUserDefaults standardUserDefaults] objectForKey:category] longLongValue];
    offlineHigh = (offlineHigh > newHigh)? offlineHigh : newHigh;
    [[NSUserDefaults standardUserDefaults] setObject:[NSNumber numberWithLongLong:offlineHigh] forKey:[NSString stringWithFormat:@"%@%@", category, offPostfix]];
}

-(int64_t) getOfflineHighScoreOfCategory:(NSString *) category {
    return [[[NSUserDefaults standardUserDefaults] objectForKey:[NSString stringWithFormat:@"%@%@", category, offPostfix]] longLongValue];
}

-(void) deleteOfflineHighScoreOfCategory:(NSString *) category {
    [[NSUserDefaults standardUserDefaults] removeObjectForKey:[NSString stringWithFormat:@"%@%@", category, offPostfix]];
}

- (void) retrieveTopScroesWithCategory:(NSString *) category {
    NSLog(@"%s",__func__);
    if([self isConnected]) {
        GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
        dispatch_async(leaderBoardQueue, ^{
            if (leaderboardRequest != nil)
            {
                leaderboardRequest.playerScope = GKLeaderboardPlayerScopeGlobal;
                leaderboardRequest.timeScope = GKLeaderboardTimeScopeAllTime;
                leaderboardRequest.category = category;
                leaderboardRequest.range = NSMakeRange(1, 5);
                [leaderboardRequest loadScoresWithCompletionHandler: ^(NSArray *scores, NSError *error) {
                    NSLog(@"Error: %@",[error localizedDescription]);
                    //NSLog(@"Scores: %@", scores);
                    if (scores && !error) {
                        NSMutableString *scoresString = [[NSMutableString alloc] initWithString:@""];
                        NSMutableArray *pIdentifierArray = [NSMutableArray array];
                        for(int i = 0; i < scores.count; i++) {
                            GKScore *leaderboardScore = [scores objectAtIndex:i];
                            if(i < scores.count - 1) [scoresString appendFormat:@"%@|", leaderboardScore.formattedValue];
                            else [scoresString appendFormat:@"%@", leaderboardScore.formattedValue];
                            [pIdentifierArray insertObject:leaderboardScore.playerID atIndex:i];
                        }
                        dispatch_async(leaderBoardQueue, ^{
                            [self retrieveTopScorersNameWithArray:pIdentifierArray scoresString:scoresString];
                        });
                    } else {
                        [self setOfflineLeaderBoardWithMessage:@"The operation couldn't be completed. There has been an Error while sending Request"];
                    }
                }];
            }
        });
    } else {
       [self setOfflineLeaderBoardWithMessage:@"No Internet Connection"];
    }
}

-(void) retrieveTopScorersNameWithArray:(NSArray *) identifiers scoresString:(NSString *) scoresString {
    NSLog(@"%s",__func__);
    if([self isConnected]) {
    [GKPlayer loadPlayersForIdentifiers:identifiers
                  withCompletionHandler:^(NSArray *players, NSError *error){
                      NSLog(@"Error:%@",[error localizedDescription]);
                      if(!error && players) {
                          //NSLog(@"Players:%@", players);
                          NSMutableString *aliasString = [[NSMutableString alloc] initWithString:@""];
                          for(int i = 0; i < players.count; i++) {
                              GKPlayer *leaderBoardPlayer = [players objectAtIndex:i];
                              if(i < players.count - 1) [aliasString appendFormat:@"%@|", leaderBoardPlayer.alias];
                              else [aliasString appendFormat:@"%@", leaderBoardPlayer.alias];
                          }
                          
                              const char * statusMessageC1 = [scoresString cStringUsingEncoding:NSStringEncodingConversionAllowLossy];
                              UnitySendMessage(UNITY_GCLISTENER_GAMEOBJECT_NAME, UNITY_GCLISTENER_ONRETRIEVETOPSCORE, statusMessageC1);
                              const char * statusMessageC2 = [aliasString cStringUsingEncoding:NSStringEncodingConversionAllowLossy];
                              UnitySendMessage(UNITY_GCLISTENER_GAMEOBJECT_NAME, UNITY_GCLISTENER_ONRETRIEVETOPSCORERSNAME, statusMessageC2);
                      } else {
                          [self setOfflineLeaderBoardWithMessage:@"The operation couldn't be completed. There has been an Error while sending Request"];
                      }
        }];
    } else {
        [self setOfflineLeaderBoardWithMessage:@"No Internet Connection"];
    }
}

-(void) setOfflineLeaderBoardWithMessage:(NSString *) message {
    const char *statusMessageC = [message cStringUsingEncoding:NSStringEncodingConversionAllowLossy];
    UnitySendMessage(UNITY_GCLISTENER_GAMEOBJECT_NAME, UNITY_GCLISTENER_DIDNOTRETRIEVEHIGHSCORES, statusMessageC);
}

#pragma mark - Achievements
-(void) resetAchievements {
    NSLog(@"%s", __func__);
    if([self isAuthenticated]) {
        [GKAchievement resetAchievementsWithCompletionHandler:^(NSError *error){
            NSLog(@"Error %@", [error localizedDescription]);
        }];
    }
}


-(void) reportAchievementsWithAchievements:(NSMutableArray *) achievementIdentifiers {
    NSLog(@"%s", __func__);
    NSLog(@"%@", achievementIdentifiers);
    __block NSMutableString *achievementsString = [[NSMutableString alloc] initWithString:@""];
    dispatch_async(achievementQueue, ^{
        for(int i = 0; i < achievementIdentifiers.count; i++) {
            //dispatch_async(achievementQueue, ^{
                GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier:[achievementIdentifiers objectAtIndex:i]];
                [achievement setShowsCompletionBanner:YES];
                achievement.percentComplete = 100.0f;
                [achievement reportAchievementWithCompletionHandler:^(NSError *error){
                     NSLog(@"%s Error: %@", __func__, [error localizedDescription]);
                    if(!error) {
                        NSLog(@"Success");
                        if(i < achievementIdentifiers.count - 1) {
                            [achievementsString appendFormat:@"%@|", achievement.identifier];
                        }
                        else {
                            [achievementsString appendFormat:@"%@", achievement.identifier];
                            const char * statusMessageC = [achievementsString cStringUsingEncoding:NSStringEncodingConversionAllowLossy];
                            UnitySendMessage(UNITY_GCLISTENER_GAMEOBJECT_NAME, UNITY_GCLISTENER_ONREPORTACHIEVEMENTS, statusMessageC);
                        }
                    }
                }];
            //});
        }
    });
}

- (void) reportAchievementIdentifier: (NSString*) identifier percentComplete: (float) percent {
    GKAchievement *achievement = [[GKAchievement alloc] initWithIdentifier: identifier];
    if (achievement)
    {
        achievement.percentComplete = percent;
        [achievement reportAchievementWithCompletionHandler:^(NSError *error)
         {
             if (error != nil)
             {
                 NSLog(@"Error in reporting achievements: %@", error);
             }
         }];
    }
}

-(void) showNotificationBannerWithTitle:(NSString *) title Message:(NSString *) message andCompletion:(void(^)(void)) completion {
    if(title && message) {
        [GKNotificationBanner showBannerWithTitle:title message:message completionHandler:completion];
    }
}

#pragma mark Delegates
-(void) gameCenterViewControllerDidFinish:(GKGameCenterViewController *)gameCenterViewController {
    [gameCenterViewController dismissViewControllerAnimated:YES completion:^{
        [self cleanViewControllerWrapper];
        [AppController UnityPause:false];
    }];
    [gameCenterViewController release];
    gameCenterViewController = nil;
}

-(void) achievementViewControllerDidFinish:(GKAchievementViewController *)viewController {
    [viewController dismissViewControllerAnimated:YES completion:^{
        [self cleanViewControllerWrapper];
        [AppController UnityPause:false];
    }];
    [viewController release];
    viewController = nil;
}

#pragma mark - ApplicationDidBecomeActive Methods

-(bool) isViewControllerWrapperPresent {
    return (self.viewControllerWrapper != nil);
}

@end