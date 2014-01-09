//
//  WebViewController.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/21/13.
//
//

#import "MoreAppsWebViewController.h"

CGFloat toolbarHeight = 44.0f;
NSString *errorMsg = @"Cannot Connect";

@interface MoreAppsWebViewController () <UIWebViewDelegate> {
    NSString *link;
}
@property (nonatomic, retain) UIWebView *webview;
@property (nonatomic, retain) UIActivityIndicatorView *activityIndicatorView;
@property (nonatomic, retain) UILabel *failureLabel;
@end

@implementation MoreAppsWebViewController
@synthesize link;
@synthesize delegate;
@synthesize activityIndicatorView;
@synthesize webview;
@synthesize failureLabel;

-(MoreAppsWebViewController *) initWithLink:(NSString *) theLink {
    self = [super init];
    if(self) {
        self.link = theLink;
    }
    return self;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    [self callWebViewWithLink:self.link];
    //[self initializeToolbar];
}

-(void) pushAlertViewWithTitle:(NSString *) title message:(NSString *) message {
    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:title message:message delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil];
    [alertView show];
    [alertView release];
}

-(void) callWebViewWithLink:(NSString *) theLink {
    webview = [[UIWebView alloc] initWithFrame:CGRectMake(0, 0, self.view.frame.size.width, self.view.frame.size.height)];
    webview.delegate = self;
    [webview loadHTMLString:@"<html><body style=\"background-color:white;\"></body></html>" baseURL:nil];
    [self performSelector:@selector(loadURL:) withObject:nil afterDelay:0.3f];
    webview.autoresizingMask = (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth);
    [self.view addSubview:webview];
}

-(void) loadURL:(id) sender {
    [webview stopLoading];
    NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:self.link]];
    [webview loadRequest:request];
}

-(void) refreshAction:(id) sender {
    if(self.webview) {
        [self.webview loadHTMLString:@"<html><head></head><body style=\"background-color:white;\"></body></html>" baseURL:nil];
        [self performSelector:@selector(loadURL:) withObject:nil afterDelay:0.3f];
    }
}

-(void) closeAction:(id) sender {
    [self dismissViewControllerAnimated:YES completion:^{
        [self.webview loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:@""]]];
        [self.webview stopLoading];
        [self.webview release];
        self.webview = nil;
        [delegate MoreAppsWebViewControllerDidFinished:self];
    }];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) showFailureMessage:(NSString *) errorMessage {
    if(failureLabel != nil) {
        [failureLabel removeFromSuperview];
        [failureLabel release];
        failureLabel = nil;
    }
    failureLabel = [[UILabel alloc] initWithFrame:CGRectMake(0, 0, 200, 20)];
    [failureLabel setBackgroundColor:[UIColor clearColor]];
    [failureLabel setNumberOfLines:1.0f];
    [failureLabel setFont:[UIFont fontWithName:@"Helvetica-Bold" size:20]];
    [failureLabel setClipsToBounds:NO];
     //failureLabel.autoresizingMask = (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
    CGSize maximumLabelSize = CGSizeMake(9999, failureLabel.frame.size.height);
    if([failureLabel respondsToSelector:@selector(setAttributedText:)]) {
        [[failureLabel text] sizeWithFont:[failureLabel font] constrainedToSize:maximumLabelSize lineBreakMode:NSLineBreakByWordWrapping];
        [failureLabel setTextAlignment:NSTextAlignmentCenter];
    } else {
        [[failureLabel text] sizeWithFont:[failureLabel font] constrainedToSize:maximumLabelSize lineBreakMode:UILineBreakModeWordWrap];
        [failureLabel setTextAlignment:UITextAlignmentCenter];
    }
    [failureLabel setCenter:CGPointMake(webview.frame.size.width / 2, webview.frame.size.height / 2)];
    [failureLabel setText:errorMessage];
    failureLabel.autoresizingMask = (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
    [webview addSubview:failureLabel];
}
-(void) webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error {
    if(activityIndicatorView != nil) {
        [activityIndicatorView stopAnimating];
        [activityIndicatorView removeFromSuperview];
        [activityIndicatorView release];
        activityIndicatorView = nil;
    }
    if(!failureLabel) {
        [self showFailureMessage:errorMsg];
    }
}

-(void) webViewDidStartLoad:(UIWebView *)webView {
    if(failureLabel != nil) {
        [failureLabel removeFromSuperview];
        [failureLabel release];
        failureLabel = nil;
    }
    if(![activityIndicatorView isAnimating]) {
        if(activityIndicatorView != nil) {
            [activityIndicatorView release];
            activityIndicatorView = nil;
        }
        activityIndicatorView =[[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
        [activityIndicatorView setCenter:CGPointMake(webView.frame.size.width * 0.5f, webView.frame.size.height * 0.5f)];
        activityIndicatorView.autoresizingMask = (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
        [webView addSubview:activityIndicatorView];
        [activityIndicatorView startAnimating];
        
    }
}

-(void) webViewDidFinishLoad:(UIWebView *)webView {
    [activityIndicatorView stopAnimating];
    [activityIndicatorView removeFromSuperview];
    [activityIndicatorView release];
    activityIndicatorView = nil;
    [failureLabel removeFromSuperview];
    [failureLabel release];
    failureLabel = nil;
    NSLog(@"Finished Loading");
}


#ifdef __IPHONE_6_0
-(BOOL)shouldAutorotate{
    return NO;
}
-(NSUInteger) supportedInterfaceOrientations {
    return UIInterfaceOrientationMaskLandscapeRight;
}
-(UIInterfaceOrientation) preferredInterfaceOrientationForPresentation {
    return UIInterfaceOrientationLandscapeRight;
}
#else
-(BOOL) shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation {
    return (toInterfaceOrientation == UIInterfaceOrientationLandscapeRight);
}
#endif

-(void) dealloc {
    [activityIndicatorView release], activityIndicatorView = nil;
    [webview release], webview = nil;
    [super dealloc];
}

@end
