//
//  CustomActivityViewController.m
//  Unity-iPhone
//
//  Created by Mikhail Lomibao Basbas on 3/4/13.
//
//

#import "CustomActivityViewController.h"

NSInteger itemsPerRow = 3;
CGFloat maxRows = 3;
CGFloat buttonSize = 60; // size is perfect square
CGFloat labelFontSize = 15;
CGFloat cancelLabelFontSize = 20;

@interface CustomActivityViewController () {
    CGRect viewFrame;
    NSInteger rows;
}
@property (nonatomic, strong) NSDictionary *imageDictionary;
@end

@implementation CustomActivityViewController
@synthesize imageDictionary = _imageDictionary;
@synthesize delegate;

-(id) initWithImageDictionary:(NSDictionary *) imageDictionary{
    self = [super init];
    if(self) {
        self.imageDictionary = imageDictionary;
        rows = [self getRows];
    }
    return self;
}

/*These are some values that requires more time to calculate, I made a hard coded one Someday I'm gonna make this fully dynamic*/

-(NSInteger) getRows {
    if(self.imageDictionary.count < 3) return 1;
    else if (self.imageDictionary.count > 3 && self.imageDictionary.count < 6) return 2;
    else return 3;
}

-(void) viewDidLoad {
    [super viewDidLoad];
    NSLog(@"%@",self.view);
    viewFrame = self.view.frame;
    [self viewSetup];
    [self initializeElements];
    
}

-(void) viewSetup {
    [self.view setOpaque:YES];
    [self.view setAlpha:1.0f];
    [self.view setBackgroundColor:[UIColor clearColor]];
    
    //Transparent background
    CGFloat canvassPositionY = (rows != maxRows) ? (viewFrame.size.height / ((CGFloat)rows + 1)) : 0.0f;
    CGFloat canvassHeight = viewFrame.size.height - canvassPositionY;
    
    UIView *canvasView = [[UIView alloc] initWithFrame:CGRectMake(0, canvassPositionY, viewFrame.size.width, canvassHeight)];
    [canvasView setBackgroundColor:[UIColor blackColor]];
    [canvasView setAlpha:0.6f];
    canvasView.autoresizingMask = ([self setAutoResizingMask] | UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth);
    [self.view addSubview:canvasView];
}


-(void) initializeElements {
    NSLog(@"Did Initialize \n ImageDictionary: %@", self.imageDictionary);
    NSEnumerator *enumerator = [self.imageDictionary keyEnumerator];
    CGSize viewSize = viewFrame.size;
    
    // Divider according to number of buttons and labels
    NSInteger divider = (self.imageDictionary.count > itemsPerRow) ? itemsPerRow + 1: self.imageDictionary.count + 1;
    // Row and column of an element
    NSInteger col = 0;
    NSInteger row = 1;
    
    //Position of an element
    CGPoint position;
    
    NSInteger tag = 0;
    
    NSString *key;
    while((key = [enumerator nextObject])) {
        NSLog(@"Key: %@", key);
        
        // Position multipliers
        if(col % 3 == 0) {
            col = 1;
            row++;
        } else col++;
        
        // Button and Label Position
        // Affected by variables row, col and divider
        position = CGPointMake((viewSize.width/ (float)divider) * (float)col, (viewSize.height / (float)divider) * (float)row);
        
        //Initialize both buttons and labels
        [self initializeButtonsWithTag:tag key:key position:position];
        [self initializeLabelsWithText:key position:position];
        
        // Button tag
        tag++;
    }
    [self initializeCancelButtonWithTag:tag viewSize:viewSize];
}

-(void) initializeCancelButtonWithTag:(NSInteger) tag viewSize:(CGSize) viewSize {
    UIButton *button = [UIButton buttonWithType:UIButtonTypeCustom];
    CGFloat buttonWidth = viewFrame.size.width * 0.7f;
    CGFloat buttonHeight = viewFrame.size.height * 0.10f;
    UIImage *image;
    if([UIImage instancesRespondToSelector:@selector(resizableImageWithCapInsets:resizingMode:)]) {
        image = [[UIImage imageNamed:@"c_button.png"] resizableImageWithCapInsets:UIEdgeInsetsMake(0, 22, 0, 22) resizingMode:UIImageResizingModeStretch];
    } else {
        image = [[UIImage imageNamed:@"c_button.png"] resizableImageWithCapInsets:UIEdgeInsetsMake(22, 22, 22, 22)];
    }
    [button setImageEdgeInsets:UIEdgeInsetsMake(0, 10, 0, 10)];
    //Setting Up the Buttons
    [button setFrame:CGRectMake(0, 0, buttonWidth, buttonHeight)];
    [button setBackgroundImage:image forState:UIControlStateNormal];
    [button setContentMode:UIViewContentModeScaleAspectFit];
    [button.titleLabel setFont:[UIFont systemFontOfSize:cancelLabelFontSize]];
    [button setTitle:@"Cancel" forState:UIControlStateNormal];
    [button setTitleColor:[UIColor whiteColor] forState:UIControlStateNormal];
    [button setTitleShadowColor:[UIColor whiteColor] forState:UIControlStateNormal];
    
    [button addTarget:self action:@selector(cancelAction:) forControlEvents:UIControlEventTouchUpInside];
    button.center = CGPointMake( viewSize.width * 0.5, viewSize.height * 0.91 );
    button.autoresizingMask = [self setAutoResizingMask] | UIViewAutoresizingFlexibleWidth;
    [self.view addSubview:button];
    
}

-(void) initializeButtonsWithTag: (NSInteger) tag key: (NSString *) key position: (CGPoint) position {
    //Create button
    UIButton *button = [UIButton buttonWithType:UIButtonTypeCustom];
    [button setFrame:CGRectMake(0, 0, buttonSize, buttonSize)];
    
    //Add image inside the dictionary
    [button setBackgroundImage:[self.imageDictionary objectForKey:key] forState:UIControlStateNormal];
    [button setContentMode:UIViewContentModeScaleToFill];
    
    // Set the tag
    button.tag = tag;
    //Set the position of the button
    button.center = position;
    //Add Target when they are tapped
    [button addTarget:self action:@selector(buttonAction:) forControlEvents:UIControlEventTouchUpInside];
    // Set their autoresizingMask when orientation is changed
    button.autoresizingMask = [self setAutoResizingMask];
    
    [self.view addSubview:button];
    
}


-(void) initializeLabelsWithText: (NSString *) text position: (CGPoint) position {
    //Set label frame, still tentative
    UILabel * label = [[UILabel alloc] initWithFrame:CGRectMake(0, 0, viewFrame.size.width * 0.3f, labelFontSize)];
    // Font size - tentative
    [label.font fontWithSize:labelFontSize];
    if([label respondsToSelector:@selector(setAttributedText:)]) {
        [[label text] sizeWithFont:[label font] forWidth:label.frame.size.width lineBreakMode:NSLineBreakByWordWrapping];
        [label setTextAlignment:NSTextAlignmentCenter];
    } else {
        [[label text] sizeWithFont:[label font] forWidth:label.frame.size.width lineBreakMode:UILineBreakModeWordWrap];
        [label setTextAlignment:UITextAlignmentCenter];
    }
    [label setText:text];
    // Text alignment - not sure if working
    // Number of lines 0 means dynamic, adjustable
    label.numberOfLines = 0;
    // background and Text Color
    label.textColor = [UIColor whiteColor];
    label.backgroundColor = [UIColor clearColor];
    
    position = CGPointMake(position.x, position.y + (buttonSize / 1.1f));
    label.center = position;
    
    label.autoresizingMask = [self setAutoResizingMask];
    
    [self.view addSubview:label];
}


-(UIViewAutoresizing) setAutoResizingMask{
    return (UIViewAutoresizingFlexibleBottomMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
}

-(void) buttonAction:(UIButton *) sender {
    [self dismissViewControllerAnimated:YES completion: ^{
        [self.delegate customActivityViewController:self tappedButtonAtIndex:sender.tag];
        self.imageDictionary = nil;
    }];
}

-(void) cancelAction:(UIButton *) sender {
    [self dismissViewControllerAnimated:YES completion:^{
        [self.delegate customActivityViewController:self didDismissWithButtonIndex:sender.tag];
        self.imageDictionary = nil;
    }];
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
    //[self.imageDictionary release];
    self.imageDictionary = nil;
    [self.delegate release], self.delegate = nil;
    [super dealloc];
}

@end