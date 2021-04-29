# CommonFeatures
WPF Tips
Limit row's height on both “Auto” and “1*”
<Grid HorizontalAlignment="Stretch"
                                              VerticalAlignment="Stretch">
                                            <Grid HorizontalAlignment="Stretch"
                                                  VerticalAlignment="Stretch">
                                                <Grid.RowDefinitions>
                                                    <RowDefinition Height="*" />
                                                    <RowDefinition Height="Auto" MinHeight="200" />
                                                </Grid.RowDefinitions>
                                                <Grid HorizontalAlignment="Stretch"
                                                      VerticalAlignment="Stretch" x:Name="HalfHeightRow" ></Grid>
                                            </Grid>
                                            <Grid>
                                                <Grid.RowDefinitions>
                                                    <RowDefinition Height="Auto" MaxHeight="{Binding ActualHeight, ElementName=HalfHeightRow}" />
                                                    <RowDefinition Height="*" />
                                                </Grid.RowDefinitions>
                                                <ContentControl Grid.Row="0"
                                                            ContentTemplate="{StaticResource FieldValueTemplate}"
                                                            Focusable="False" IsTabStop="False">
                                                    <ContentControl.Content>                                                       

                                                    </ContentControl.Content>
                                                </ContentControl>

                                                <ContentControl Grid.Row="1"
                                                            ContentTemplate="{StaticResource ToolTipTemplate}"
                                                            Content="{Binding}" Focusable="False" IsTabStop="False" />
                                            </Grid>
                                        </Grid>
