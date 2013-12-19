#import <UIKit/UIKit.h>

@interface AppController : NSObject<UIAccelerometerDelegate, UIApplicationDelegate>
{
}
- (void) startUnity:(UIApplication*)application;
- (void) Repaint;
- (void) RepaintDisplayLink;
- (void) prepareRunLoop;
+ (void) UnityPause:(bool) value;
@end

int		UnityGetAccelerometerFrequency();
